
# PPTDriveManager
## Overview
A simple web application to upload PPT files to Google Drive and MS SQL database, managed through a .NET Core backend and React frontend. Features include multi-file upload, viewing, and deleting presentations, with integrated Google Drive API support for file handling.

## Features
- Upload PPT files and save them to Google Drive.
- View uploaded files with a preview option.
- Download and delete files.
- Responsive design with a user-friendly interface.

## Getting Started

### Prerequisites
- Node.js (for React frontend)
- .NET Core SDK (for backend)
- Google Drive API credentials

#Project Structure
   1. DotNetCoreWebApi
      - Only PPTApi Folder
   2. React Frontend Part
      - public, src Folder and other files


# Setting Up OAuth Credentials

To create your Client ID, Client Secret ID, and Refresh Token, follow these steps:

1. **Watch the Tutorial Video:**
   Follow this [YouTube tutorial](https://youtu.be/1y0-IfRW114?si=Qq-KrCOWdtW6buAW) which provides a detailed walkthrough of the entire process.

2. **Steps Summary:**
   - **Create a Project:** Go to the Google Cloud Console and create a new project.
   - **Enable APIs:** Enable the necessary APIs for your project.
   - **Create OAuth Credentials:** Navigate to the credentials page and create new OAuth 2.0 Client IDs.
   - **Download Credentials:** Download the credentials file which will contain your Client ID and Client Secret ID.
   - **Generate Refresh Token:** Use the OAuth 2.0 Playground to generate a refresh token using your Client ID and Client Secret ID.

3. **Important Notes:**
   - Keep your Client ID, Client Secret ID, and Refresh Token secure and not publicly share them.
   - Make sure to follow best practices for storing and using these credentials in your application.

