﻿using CertificatesWebApp.Users.Repositories;
using Data.Models;
using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Dtos;
using CertificatesWebApp.Exceptions;

namespace CertificatesWebApp.Users.Services
{
    public interface ICredentialsService : IService<Credentials>
    {
        Credentials CreateCredentials(Credentials credentials);
        void Authenticate(String email, String password);
    }
    public class CredentialsService : ICredentialsService
    {
        private readonly ICredentialsRepository _credentialsRepository;

        public CredentialsService(ICredentialsRepository credentialsRepository)
        {
            _credentialsRepository = credentialsRepository;
        }

        public Credentials CreateCredentials(Credentials credentials)
        {
            return _credentialsRepository.Create(credentials);
        }

        public void Authenticate(String email, String password)
        {
            Credentials credentials = _credentialsRepository.findCredentials(email);
            if (credentials == null || credentials.Password != password)
            {
                throw new BadCredentialsException("Email or password is incorrect!");
            }
            else if (!credentials.User.IsActivated)
            {
                throw new UserNotActivatedException("User is not activated!");
            }
        }
    }
}
