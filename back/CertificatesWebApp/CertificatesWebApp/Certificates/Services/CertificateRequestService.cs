 using CertificatesWebApp.Certificates.Repositories;
using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Certificates.DTOs;
using Data.Models;
 using Microsoft.Identity.Client;
using CertificatesWebApp.Exceptions;

namespace CertificatesWebApp.Users.Services
{
    public interface ICertificateRequestService : IService<Data.Models.CertificateRequest>
    {
        Task MakeRequestForCertificate(Guid userId, String role, CertificateRequestDTO dto);
        Data.Models.CertificateRequest GetCertificateRequest(Guid certificateRequestId);
        List<GetCertificateRequestDTO> GetAll();
        List<GetCertificateRequestDTO> GetAllPagable(PageParametersDTO pageParameters);
        Task<List<GetCertificateRequestDTO>> GetAllForUser(Guid userId);
        Task<List<GetCertificateRequestDTO>> GetAllForUserPagable(PageParametersDTO pageParameters, Guid userId);
        Task<List<GetCertificateRequestDTO>> GetAllForApproval(Guid guid);
        Task<List<GetCertificateRequestDTO>> GetAllForApprovalPagable(PageParametersDTO pageParameters, Guid guid);


    }
    public class CertificateRequestService : ICertificateRequestService
    {
        private readonly ICertificateRequestRepository _certificateRequestRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICertificateService _certificateService;
        private readonly IUserService _userService;
        private readonly ILogger<CertificateRequestService> _logger;

        public CertificateRequestService(ICertificateRequestRepository certificateRequestRepository, ICertificateService certificateService, 
            ICertificateRepository certificateRepository, IUserService userService, ILogger<CertificateRequestService> logger)
        {
            _certificateRequestRepository = certificateRequestRepository;
            _certificateRepository = certificateRepository;
            _certificateService = certificateService;
            _userService = userService;
            _logger = logger;
        }

        public async Task MakeRequestForCertificate(Guid userId, String role, CertificateRequestDTO dto)
        {
            checkDto(dto, userId);
            // ako je root ne moramo da proveravamo validnost parenta
            if (dto.Type != CertificateType.ROOT && !(await checkValidity(dto.EndDate, dto.ParentSerialNumber)))
            {
                _logger.LogError("User {@Id} failed to make request for certificate due to invalid request", userId);
                throw new InvalidInputException("Invalid certificate request!");
            }
            if(role == "Admin")
            {
                adminMakesRequests(userId, dto);
            }
            else
            {
                await userMakesRequests(userId, dto);
            }
        }

        private void checkDto(CertificateRequestDTO dto, Guid userId)
        {
            if (dto.EndDate < DateTime.Now)
            {
                _logger.LogError("User {@Id} failed to make request for certificate due to invalid end date", userId);
                throw new InvalidInputException("Cannot make certificate for past!");
            }

            if (dto.Type == CertificateType.ROOT && !dto.ParentSerialNumber.Equals(""))
            {
                _logger.LogError("User {@Id} failed to make request for root certificate based on other certificte", userId);
                throw new InvalidInputException("Cannot make root certificate based on other certificate!");
            }

            if (dto.Type != CertificateType.ROOT && dto.ParentSerialNumber.Equals(""))
            {
                _logger.LogError("User {@Id} failed to make request for self signed non root certificate", userId);
                throw new InvalidInputException("Cannot make certificate on its own!");
            }

            if (dto.Type == CertificateType.END && dto.Flags.Contains("2"))
            {
                _logger.LogError("User {@Id} failed to make request for end certificate due to wrong flags included", userId);
                throw new InvalidInputException("End certificate cannot have this permissions!");
            }

            if (dto.Type != CertificateType.END && !dto.Flags.Contains("2"))
            {
                _logger.LogError("User {@Id} failed to make request for root or end certificate due to wrong flags included", userId);
                throw new InvalidInputException("This type of certificate must include 4th flag");
            }
        }

        private async Task<Boolean> checkValidity(DateTime endDate, String serialNumber)
        {
            Certificate certificate = await _certificateRepository.FindBySerialNumber(serialNumber);
            if(certificate == null)
            {
                throw new ResourceNotFoundException("Certificate does not exist!");
            }

            if (certificate.Type == CertificateType.END)
                return false;

            if (DateTime.UtcNow > endDate)
                return false;

            if (!_certificateService.IsValid(serialNumber)) {
                return false;
            }

            if (certificate.EndDate < endDate)
                return false;
            return true;
        }

        private void adminMakesRequests(Guid userId, CertificateRequestDTO dto)
        {
            Data.Models.CertificateRequest request = saveRequest(userId, dto);
            _certificateService.AcceptCertificate(request.Id);
        }

        private async Task userMakesRequests(Guid userId, CertificateRequestDTO dto)
        {
            if(dto.Type == CertificateType.ROOT)
            {
                _logger.LogError("User {@Id} do not have persmission to make root certiificates", userId);
                throw new InvalidInputException("You do not have permission to make root certificates!");
            }
            if(await amIOwner(userId, dto.ParentSerialNumber))
            {
                Data.Models.CertificateRequest request = saveRequest(userId, dto);
                _certificateService.AcceptCertificate(request.Id);
            }
            else
            {
                _ = saveRequest(userId, dto);
            }

        }

        private async Task<Boolean> amIOwner(Guid userId, String serialNumber)
        {
            Certificate parentCertificate = await _certificateRepository.FindBySerialNumber(serialNumber);
            if(parentCertificate == null)
            {
                _logger.LogError("User {@Id} failed to find certificate {@SerialNumber}", userId, serialNumber);
                throw new InvalidInputException("Certificate does not exist!");
            }
            return userId == parentCertificate.OwnerId;
        }

        

        private Data.Models.CertificateRequest saveRequest(Guid userId, CertificateRequestDTO dto)
        {
            Data.Models.CertificateRequest certificateRequest = new Data.Models.CertificateRequest();
            certificateRequest.State = CertificateRequestState.IN_PROGRESS;
            certificateRequest.Type = dto.Type;
            certificateRequest.ParentSerialNumber = dto.Type != CertificateType.ROOT ? dto.ParentSerialNumber : "";
            certificateRequest.SubjectText = "";
            if (!dto.O.Equals("")) certificateRequest.SubjectText += $"O={dto.O};";
            if (!dto.OU.Equals("")) certificateRequest.SubjectText += $"OU={dto.OU};";
            if (!dto.C.Equals("")) certificateRequest.SubjectText += $"C={dto.C};";
            Console.Write(certificateRequest.SubjectText);
            certificateRequest.EndDate = dto.EndDate;
            certificateRequest.Flags = dto.Flags;
            certificateRequest.HashAlgorithm = dto.HashAlgorithm;
            certificateRequest.OwnerId = userId;

            return _certificateRequestRepository.Create(certificateRequest);
        }

        public Data.Models.CertificateRequest Get(Guid certificateRequestId)
        {
            return _certificateRequestRepository.Read(certificateRequestId);
        }

        public Data.Models.CertificateRequest Update(Data.Models.CertificateRequest certificateRequest)
        {
            return _certificateRequestRepository.Update(certificateRequest);
        }

        public Data.Models.CertificateRequest GetCertificateRequest(Guid certificateRequestId) { 
            return _certificateRequestRepository.Read(certificateRequestId);
        }

        public async Task<List<GetCertificateRequestDTO>> GetAllForUser(Guid userId)
        {
            List<Data.Models.CertificateRequest> certificateRequests = await _certificateRequestRepository.FindByUserId(userId);
            return certificateRequests.Select(x => new GetCertificateRequestDTO(x)).ToList();
        }

        public List<GetCertificateRequestDTO> GetAll()
        {
            List<Data.Models.CertificateRequest> certificateRequests = (List<Data.Models.CertificateRequest>)_certificateRequestRepository.ReadAll();
            return certificateRequests.Select(x => new GetCertificateRequestDTO(x)).ToList();
        }

        public async Task<List<GetCertificateRequestDTO>> GetAllForApproval(Guid guid)
        {
            IEnumerable<Certificate> certificates = await _certificateRepository.FindByOwnerId(guid);
            List<Data.Models.CertificateRequest> requests = new List<Data.Models.CertificateRequest>();
            foreach (Certificate certificate in certificates)
            {
                List<Data.Models.CertificateRequest> foundCertificatesRequests = await _certificateRequestRepository.FindByParentSerialNumber(certificate.SerialNumber);
                requests.AddRange(foundCertificatesRequests);
            }
            return requests.Select(x => new GetCertificateRequestDTO(x)).ToList();

        }

        public List<GetCertificateRequestDTO> GetAllPagable(PageParametersDTO pageParameters)
        {
            List<Data.Models.CertificateRequest> certificateRequests = _certificateRequestRepository.ReadAll().Skip((pageParameters.PageNumber - 1) * pageParameters.PageSize).Take(pageParameters.PageSize).ToList();
            return certificateRequests.Select(x => new GetCertificateRequestDTO(x)).ToList();
        }

        public async Task<List<GetCertificateRequestDTO>> GetAllForApprovalPagable(PageParametersDTO pageParameters, Guid guid)
        {
            IEnumerable<Certificate> certificates = await _certificateRepository.FindByOwnerId(guid);
            List<Data.Models.CertificateRequest> requests = new List<Data.Models.CertificateRequest>();
            foreach (Certificate certificate in certificates)
            {
                List<Data.Models.CertificateRequest> foundCertificatesRequests = await _certificateRequestRepository.FindByParentSerialNumber(certificate.SerialNumber);
                requests.AddRange(foundCertificatesRequests);
            }
            return requests.Select(x => new GetCertificateRequestDTO(x)).Skip((pageParameters.PageNumber - 1) * pageParameters.PageSize).Take(pageParameters.PageSize).ToList(); ;
        }
        public async Task<List<GetCertificateRequestDTO>> GetAllForUserPagable(PageParametersDTO pageParameters, Guid userId)
        {
            List<Data.Models.CertificateRequest> certificateRequests = await _certificateRequestRepository.FindByUserId(userId);
            return certificateRequests.Select(x => new GetCertificateRequestDTO(x)).Skip((pageParameters.PageNumber - 1) * pageParameters.PageSize).Take(pageParameters.PageSize).ToList();
        }


    }
}