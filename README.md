<div id="top"></div>

<!-- PROJECT LOGO -->

# CZ4010 Cryptography Project

<h3 align="center">Secure File Sharing System</h3>


## Team "404 Not Found"
- [Reenashini Rajendran](https://github.com/reenaxxraj)
- [Ong Yu Hann](https://github.com/Archeri2000)

<!-- Motivation -->
## Motivation
Cloud services are a power tool that has enabled many organisations to streamline their IT processeses. It enables users to store files and applications on remote servers which allows businesses to retain their applications and business processes without having to deal with the backend technicalities. Cloud computing has numerous benefits to it. It facilitates easy back-up and recovery, which is otherwise costly to be implemented on-premise. It also helps users located in different geographies to collaborate in a  convenient manner.

Nevertheless, these advantages can compromise on the security of the information hosted in the cloud. Sharing the company’s sensitive information to a third-party cloud computing service provider is risky. Clouds are “black boxes” that the organisation cannot directly monitor or manage. The cloud provider owns the physical premises, hardware, software and networks that our information is being hosted in. These resources are not dedicated to any single customer and we do not know who has access to it. Hence, we cannot be certain that the security measures the provider has in place is enough to protect our data.

Therefore, we would like to come up with a solution that allows us to leverage the benefits of a cloud service provider while mitigating the security risks to a certain extent. In this project we will explore how we can develop a secure file sharing system that allows us to share files through an untrusted cloud service provider.

<!-- Research -->
## Research
**Considerations**:
- Server should never be allowed to possess plaintext of files or any keys
- Key distribution should be possible without exposing any secrets to the server and without needing both parties to be online and active
- Tampering of the file need to be detectable by end users
- Any actions taken should ideally be logged and undisputable

**Basic Concept**:

Asymmetric key cryptography should be used to establish identities and prove ownership, but symmetric key cryptography is likely better for the actual file encryption. By having clients handle all the encryption and decryption, the File Server merely acts as an abstraction over the database and a validator of requests.

<!-- Design -->
## Design

### Security goals of our application:

* Confidentiality

A key goal of our application is to keep the contents of each file in the system confidential by only allowing users with the appropriate access rights to read the contents of the file. To implement this we will use a shared symmetric key to encrypt each file. This key will be only be shared with the users who are given read rights by the owner of the file. This shared key will be encrypted using the public key of users that have been given read access rights and stored in the database. Both the encrypted shared key and encrypted file will be stored in the database. This allows users to retrieve the encrypted file and encrypted shared key from the server at their own convenience and read the contents of the file.

* Integrity

Encrypted files uploaded to the cloud are still susceptible to tampering by a malicious. Therefore, we will have to ensure that client is aware when the file has been modified by someone else other than the owner of the file (Only owner is given permission to modify the file). The decrypted file is signed by the owner before being encrypted, and this provides an additional layer of verification during the decryption process. Even if the decryption key was stolen and used to modify the plaintext file, the signature would not match, thus signalling that the file integrity has been compromised.

* Authentication

Each user in the system is identified using their UserID, and requests to the server are all signed using the user's private key. This signature can be verified by querying the trusted Identity Server to obtain the user's public key. 

* Non-repudiation

In order to prevent any users from disputing the modifications they have made to the file, we have used audit logs of every signed request to keep track of every request made by each user to to the server.

### Functions of our application

![image](https://user-images.githubusercontent.com/44928185/143735599-de2cc681-2f4d-4765-aa6d-7a2047502910.png)

![image](https://user-images.githubusercontent.com/44928185/143735618-108b80b9-f558-42de-a17d-1fb0db196e71.png)

![image](https://user-images.githubusercontent.com/44928185/143735584-9f255259-6033-4b8d-9d86-816b13b81bea.png)





<!-- Development -->
## Development
Our system has been split into 2 different applications. A client side app which will be used by the users. A server side app which will act as a middle man between the client app and the database.

### Client side application

Developed as C# .NET Console Application.
Used cryptography package provided by .NET (System.Security.Cryptography)
Asymmetric Encryption => RSA-2048
Symmetric Encryption => AES-256 in CFB (Cipher Feedback) mode + Randomly generated IV

### Server side application
The server side application utilises the following:
- .NET Cryptography library
- POSTGRES Database
- ASP.NET Framework

It is designed as two separate components under different security assumptions. Owing to the need for a fundamental root of trust within the system, an Identity Server is deemed trustworthy and is the central source of verification. It keeps track of the public keys of all users and links it to their username. The File Server is assumed to be potentially untrustworthy and is never allowed to process plaintext or unencrypted keys.
<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these simple example steps.

### Prerequisites

This is an example of how to list things you need to use the software and how to install them.
* Installation of Docker
  ```sh
  Install from https://docs.docker.com/desktop/windows/install/
  ```
* Visual Studio with .NET Framework
  ```sh
  Install from https://docs.microsoft.com/en-us/dotnet/framework/install/guide-for-developers
  ```

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/reenaxxraj/CZ4010AppliedCryptoProject.git
   ```
2. Start the server application
   ```sh
   cd CZ4010AppliedCryptoProject/CZ4010-Server
   docker-compose build
   docker-compose up
   ```
3. Start the client application in either Visual Studio or Jetbrains Rider.

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- SECURITY GUARANTEES -->
## Security Guarantees

We would like to provide a brief analysis of the security guarantees each of the core functionalities provide.

- **UploadFile**
    - Honest Conditions (Guarantee "Liveness")
        - Successful upload of encrypted file
        - Encrypted file is also signed to prove ownership
    - Malicious Conditions (Guarantee "Safety")
        - File may not be stored on the server, but is guaranteed to stay encrypted regardless
        - Server has no access to symmetric key needed to decrypt
- **DownloadFile**
    - Honest Conditions (Guarantee "Liveness")
        - Successful download of encrypted file and key
        - Encrypted file can be decrypted
        - Unauthorised users cannot download the file, nor do they have a key
    - Malicious Conditions (Guarantee "Safety")
        - Unauthorised users may download the file, but have no key to decrypt it
        - Server may deny the file to authorised users
- **ModifyFile**
    - Honest Conditions (Guarantee "Liveness")
        - Old file will be replaced by new file
        - Both are encrypted, people who could access the old one can access the new one
        - Only file owner can modify
    - Malicious Conditions (Guarantee "Safety")
        - Old file may not be replaced
        - New file may not be uploaded
        - Files remain encrypted
        - Other users may "modify" even though they are not the file owner, but signature verification of the encrypted file will fail.
- **ShareFile**
    - Honest Conditions (Guarantee "Liveness")
        - File will successfully be shared with users specified
    - Malicious Conditions (Guarantee "Safety")
        - File may not be shared with specified users
        - Regardless, file will not be able to be decrypted by unspecified users
- **UnshareFile**
    - Honest Conditions (Guarantee "Liveness")
        - File will be denied to users specified
        - Specified users' keys will be deleted
    - Malicious Conditions (No Guarantee)
        - Server may refuse to delete specified users' keys allowing them to continue accessing the file.
- **DeleteFile**
    - Honest Conditions (Guarantee "Liveness")
        - File will be deleted
        - All keys will also be deleted
    - Malicious Conditions (No Guarantee)
        - File may not be deleted
        - Users may still be able to access file
- **Logging**
    - Honest Conditions (Guarantee "Liveness")
        - Logs accurately reflect the actions of users
    - Malicious Conditions (Partially Guarantee "Safety")
        - Logs may be deleted by the server even though the user made the request
        - Any log that does exist is guaranteed to be real as server cannot forge user signatures on the request

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/github_username/repo_name.svg?style=for-the-badge
[contributors-url]: https://github.com/github_username/repo_name/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/github_username/repo_name.svg?style=for-the-badge
[forks-url]: https://github.com/github_username/repo_name/network/members
[stars-shield]: https://img.shields.io/github/stars/github_username/repo_name.svg?style=for-the-badge
[stars-url]: https://github.com/github_username/repo_name/stargazers
[issues-shield]: https://img.shields.io/github/issues/github_username/repo_name.svg?style=for-the-badge
[issues-url]: https://github.com/github_username/repo_name/issues
[license-shield]: https://img.shields.io/github/license/github_username/repo_name.svg?style=for-the-badge
[license-url]: https://github.com/github_username/repo_name/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/linkedin_username
[product-screenshot]: images/screenshot.png
