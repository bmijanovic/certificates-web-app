<img src="https://github.com/bmijanovic/certificates-web-app/assets/51921035/6b81419c-b690-4846-b09e-69debe482cdd" width="150px"/>
<br/>


# Certificates Web App
Certificates Web App is a secure and efficient platform designed for creating and managing certificates. Its backend is built on .NET 6, while the web frontend is powered by React JS. The application enables users to handle certificate generation, validation, and more.

To ensure data security, private keys are stored locally, while public certificate information is stored in the database. The app offers various security features, including OAuth 2.0 for secure login. We implemented password recovery, 2FA and password rotation for enhanced protection.

To monitor and track activities, a logger is implemented, and sessions are efficiently tracked. ReCAPTCHAv3, SendGrid, and Twilio are integrated to reinforce security and communication.

Data validation is enforced throughout the app, which is fortified against common attacks such as injection, XSS, and path traversal attacks.
 
## Backend
The backend of the Certificates Web App is built on .NET 6 to ensure optimal performance and modern features. A key aspect of our backend architecture is the utilization of the MSSQL database, which provides robust and reliable data storage for the application.

Our primary focus for this application is security. We have implemented stringent security measures to safeguard sensitive data and prevent unauthorized access. Security protocols, such as encryption, authentication, and authorization, are rigorously employed to protect user information and ensure the integrity of the certificates managed within the system.

## Web Frontend
The web frontend of the Certificates Web App is built on React JS and utilizes Material UI for a minimalistic design. Security is our top priority, and the clean interface allows users to focus solely on certificate management without distractions. The combination of React JS and Material UI ensures a user-friendly experience, making certificate management efficient and effective for all users.

![image](https://github.com/bmijanovic/certificates-web-app/assets/51921035/9c2b7017-11f7-46bc-aaf0-e22b7237a90a)

## Features

### Login, Registration & Oauth
The login and registration process in our system is designed with security and user convenience in mind. To register, a user must fill in a valid form with required information. After registration, the user's profile is verified by entering a code sent to their chosen email or phone number. This additional verification step ensures the authenticity of user accounts.

Once registered and verified, the user can log in to their account. We have implemented Two-Factor Authentication (2FA) for an extra layer of security. After entering their login credentials, the user must confirm their identity by entering a code sent to their email or phone number. This 2FA process adds an additional security measure to protect user accounts from unauthorized access.

To ensure session security, we have set a session time after which the user must log in again. Cookies are used for session tracking, allowing for smooth and secure user sessions while browsing the application.

Moreover, we provide an alternative method for login and registration using OAuth 2.0 protocol. Users have the option to use their Google account for login or registration, simplifying the process and providing a seamless user experience

![image](https://github.com/bmijanovic/certificates-web-app/assets/51921035/c51b9ef2-76c4-453b-8895-88fcf87807da)

### Password Rotation, Account Recovery

The Certificates Web App ensures a robust and secure password recovery and password rotation process, prioritizing the protection of user accounts and sensitive information.

For password recovery, if a user forgets their password, they can enter their registered email or phone number. If the provided credentials exist, the system generates a unique code and sends it to the user's email or phone number. Once the user receives the code, they can enter it into the system to gain access to a new password setup form. This process enables users to regain access to their account while ensuring the user's identity is verified through a separate communication channel.

Password rotation is a key security feature that prompts users to change their passwords after a certain period. This measure ensures that passwords are regularly updated, reducing the risk of unauthorized access due to long-term exposure. By encouraging regular password changes, the Certificates Web App enhances the overall security posture of user accounts.

Additionally, to reinforce password security, the system stores the previous N passwords (only their salted hash) for each user. This mechanism prevents users from using previously used passwords, adding an extra layer of protection against password reuse and potential vulnerabilities.

### Create Certificate Request, Accepting & Rejecting Certificate Requests
In the Certificates Web App, all users have the ability to create certificate requests, depending on the type of certificate they wish to generate. Specifically, users can create requests for intermediate or end certificates. However, only administrators are permitted to create root certificates due to their critical nature in the certificate hierarchy.

When a user creates a certificate request for an intermediate or end certificate, the request is submitted to the owner of the parent certificate. The parent certificate owner, must approve the request before the certificate is issued.

To streamline the process, certain scenarios are automatically accepted. If the person who created the request and the owner of the parent certificate are the same user, the request is automatically approved, as there is a direct association between the requester and the parent certificate.

Similarly, if an admin creates a certificate request, it is automatically accepted, as admins possess the authority to issue certificates without additional approval.

When creating a new certificate request, users are required to specify various details, including the type of certificate (i.e., intermediate or end), the serial number of the parent certificate if applicable, additional information about the certificate owner, the desired expiration date, and key flags to configure access rights and security settings.

![image](https://github.com/bmijanovic/certificates-web-app/assets/51921035/3bf70700-bab4-4a7f-aab2-2cc2b1c4199c)

In the Certificates Web App, users have the capability to view their own certificate requests, and administrators have access to view all certificates within the system.

When a user creates a certificate request, it is submitted to the owner of the parent certificate for approval. The owner can either accept or reject the request. If the owner rejects the request, they are required to provide a reason for the rejection.

If the owner accepts the certificate request, the certificate is automatically created and becomes valid. This streamlined process ensures that valid certificate requests are promptly processed and issued, simplifying the workflow for users and administrators alike.

![image](https://github.com/bmijanovic/certificates-web-app/assets/51921035/c56ceb50-e3a4-4d03-aa62-00e351f09694)

### Preview, Download, Check Validity & Revoking Certificates

The Certificate Preview allows all users to access and view certificates along with their relevant information. Users can easily see essential details, such as certificate validity, date of creation, expiration date, public information about the owner, and the certificate serial number.

With this feature, users gain a comprehensive overview of all certificates within the system, promoting transparency and easy access to critical information.

![image](https://github.com/bmijanovic/certificates-web-app/assets/51921035/1cafc2b4-29dd-4a40-b7ea-1ddbc5e6d6a8)

All users have the privilege to download the public part of certificates, facilitating easy access to relevant public information. This feature allows users to verify the authenticity and validity of certificates, ensuring transparency and trust in the system.

For added security, only the owner of a certificate has the authorization to download the private key associated with their specific certificate. This measure ensures that sensitive information remains protected and accessible only to the rightful owner.

Moreover, the application allows users to download all certificates, even if they are revoked or expired. This capability offers users the ability to maintain a historical record of certificates and supports administrative purposes, ensuring a comprehensive overview of certificate management within the system.

![image](https://github.com/bmijanovic/certificates-web-app/assets/51921035/d5a64aa6-57b5-4e92-9de9-015a1e54bd18)

In the Certificates Web App, all users can easily check the validity of certificates through two convenient methods. First, users can enter the certificate's serial number into the system. By doing so, the application instantly verifies the certificate's validity status, providing prompt feedback to the user.

Alternatively, users can also verify the certificate's validity by uploading a copy of the certificate. The application performs an automated analysis of the uploaded document, extracting the necessary information to check the certificate's authenticity and validity.

![image](https://github.com/bmijanovic/certificates-web-app/assets/51921035/3b2e8912-ace3-4316-9472-eb883b812fd8)

The owners of certificates possess the option to revoke their certificates when necessary. Upon revoking a certificate, the system enforces recursive revocation, meaning that all certificates issued by the revoked certificate are also automatically revoked.

This recursive revocation ensures that the revocation status is propagated to all certificates linked to the original certificate, effectively invalidating their authenticity. This approach maintains data integrity and reinforces security measures, preventing the use of certificates that may have relied on the revoked one for their validity.

### ReCAPTCHA, HTTPS & Other

The Certificates Web App has been fortified with robust security measures to safeguard against various threats. To protect against spam, ReCAPTCHAv3 is implemented both on the frontend and backend, ensuring a human presence during form submissions.

Security vulnerabilities like Injection, XSS, and Path Traversal attacks have been addressed through comprehensive measures, bolstering the application's resilience against potential exploits.

Data validation is enforced throughout the application to ensure that only verified and accurate information is processed, reducing the risk of data corruption or manipulation.

To enhance security, private keys are stored locally, keeping them safe from unauthorized access. Only public information is stored in the database, minimizing exposure of sensitive data.

A logger is integrated into the system for monitoring purposes, allowing the team to track and analyze events for troubleshooting and continuous improvement.

To mitigate the vulnerabilities associated with the SMTP protocol, the application uses SendGrid, a reliable and secure email delivery service.

