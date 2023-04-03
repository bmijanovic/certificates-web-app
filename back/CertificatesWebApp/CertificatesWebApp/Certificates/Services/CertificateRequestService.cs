using CertificatesWebApp.Certificates.Repositories;
using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Certificates.DTOs;
using Data.Models;

namespace CertificatesWebApp.Users.Services
{
    public interface ICertificateRequestService : IService<Data.Models.CertificateRequest>
    {
        Task MakeRequestForCertificate(Guid userId, String role, CertificateRequestDTO dto);
        Data.Models.CertificateRequest GetCertificateRequest(Guid certificateRequestId);
        Task<List<GetCertificateRequestDTO>> GetAllForUser(Guid userId);
    }
    public class CertificateRequestService : ICertificateRequestService
    {
        private readonly ICertificateRequestRepository _certificateRequestRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICertificateService _certificateService;

        public CertificateRequestService(ICertificateRequestRepository certificateRequestRepository, ICertificateService certificateService, 
            ICertificateRepository certificateRepository)
        {
            _certificateRequestRepository = certificateRequestRepository;
            _certificateRepository = certificateRepository;
            _certificateService = certificateService;
        }

        public async Task MakeRequestForCertificate(Guid userId, String role, CertificateRequestDTO dto)
        {
            checkDto(dto);
            // ako je root ne moramo da proveravamo validnost parenta
            if (dto.Type != CertificateType.ROOT && !(await checkValidity(dto.EndDate, dto.ParentSerialNumber)))
                throw new Exception("Invalid certificate request!");
            if(role == "Admin")
            {
                adminMakesRequests(userId, dto);
            }
            else
            {
                userMakesRequests(userId, dto);
            }
        }

        private void checkDto(CertificateRequestDTO dto)
        {
            if (dto.EndDate < DateTime.Now)
                throw new Exception("Cannot make certificate for past!");
            if (dto.Type == CertificateType.ROOT && !dto.ParentSerialNumber.Equals(""))
                throw new Exception("Cannot make root certificate based on other certificate!");
            if (dto.Type != CertificateType.ROOT && dto.ParentSerialNumber.Equals(""))
                throw new Exception("Cannot make certificate on its own!");
            if (dto.Type == CertificateType.END && dto.Flags.Contains("4"))
                throw new Exception("End certificate cannot have this permissions!");
        }

        private async Task<Boolean> checkValidity(DateTime endDate, String serialNumber)
        {
            Certificate certificate = await _certificateRepository.FindBySerialNumber(serialNumber);
            if(certificate == null)
            {
                throw new Exception("Certificate does not exist!");
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
                throw new Exception("You do not have permission to make root certificates!");
            }
            if(await amIOwner(userId, dto.ParentSerialNumber))
            {
                Data.Models.CertificateRequest request = saveRequest(userId, dto);
                _certificateService.AcceptCertificate(request.Id);
            }
            else
            {
                saveRequest(userId, dto);
            }

        }

        private async Task<Boolean> amIOwner(Guid userId, String serialNumber)
        {
            Certificate parentCertificate = await _certificateRepository.FindBySerialNumber(serialNumber);
            if(parentCertificate == null)
            {
                throw new Exception("Certificate does not exist!");
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
            List<CertificateRequest> certificateRequests = await _certificateRequestRepository.FindByUserId(userId);
            return certificateRequests.Select(x => new GetCertificateRequestDTO(x)).ToList();
        }
    }
}