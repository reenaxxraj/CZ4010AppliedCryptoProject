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

<!-- Overview -->
## Overview
**Considerations**:
- Server should never be allowed to possess plaintext of files or any keys
- Key distribution should be possible without exposing any secrets to the server and without needing both parties to be online and active
- Tampering of the file need to be detectable by end users
- Any actions taken should ideally be logged and undisputable

**Basic Concept**:

Asymmetric key cryptography should be used to establish identities and prove ownership, but symmetric key cryptography is likely better for the actual file encryption. By having clients handle all the encryption and decryption, the File Server merely acts as an abstraction over the database and a validator of requests.

**Summary**:

All files are signed by their owner and the file + signature is then encrypted using a AES symmetric key. The symmetric key is then encrypted using the owner's RSA public key. The encrypted key and file are then sent to the file server for storage. Whenever any user needs to retrieve a file, they will retrieve the encrypted file and their personal encrypted key from the server, decrypting the key using their RSA private key. The use the decrypted AES key to decrypt the file, verifying the owner's signature in the process. To share the file with other users, the owner downloads their encrypted key, decrypting it using their RSA private key to obtain the AES key. This AES key is then encrypted using the public key of each user the file is to be shared with, creating a unique encrypted key for each user. These keys are then uploaded to the file server for storage.

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
**Create Account**
![image](https://user-images.githubusercontent.com/16084793/143774089-00a043c8-3875-48d6-a8dd-ded0ae9c6f33.png)

**Login**
![image](https://user-images.githubusercontent.com/16084793/143774139-a13e6bdd-4231-472d-85b1-537aa37da28b.png)

**Upload File**
![image](https://user-images.githubusercontent.com/16084793/143774205-dd4d8f1e-2bd8-49b6-8bb2-abc848115266.png)

**Download File**
![image](https://user-images.githubusercontent.com/16084793/143774258-1a4d6b79-1ed3-414b-9a30-d40d1ebb2e3d.png)

**Modify File**
![image](https://user-images.githubusercontent.com/16084793/143774308-86f52a5a-59fe-4e4d-aaac-5b7f176db9b7.png)

**Delete File**
![image](https://user-images.githubusercontent.com/16084793/143774362-c425502f-0297-4f85-b63c-85f087ba590a.png)

**Share File**
![image](https://user-images.githubusercontent.com/16084793/143774401-460d05d8-8cec-4043-95b0-4068a581e37f.png)

**Unshare File**
![image](https://user-images.githubusercontent.com/16084793/143774455-6cc5d65a-0078-44b3-987c-eb5701c4f230.png)

**Get Logs**
![image](https://user-images.githubusercontent.com/16084793/143774481-da41827c-94f4-4e68-b86f-994916b79a71.png)




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

