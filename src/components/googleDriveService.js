// const fs = require('fs');
// const { google } = require('googleapis');
// const path = require('path');
// const CLIENT_ID = '';
// const CLIENT_SECRET = '';
// const REDIRECT_URI = 'https://developers.google.com/oauthplayground';
// const REFRESH_TOKEN = '';

// const oauth2Client = new google.auth.OAuth2(
//   CLIENT_ID,
//   CLIENT_SECRET,
//   REDIRECT_URI
// );

// oauth2Client.setCredentials({ refresh_token: REFRESH_TOKEN });

// const drive = google.drive({
//   version: 'v3',
//   auth: oauth2Client,
// });

// async function uploadFile(filePath) {
//   try {
//     const response = await drive.files.create({
//       requestBody: {
//         name: path.basename(filePath),
//         mimeType: 'application/vnd.openxmlformats-officedocument.presentationml.presentation',
//       },
//       media: {
//         mimeType: 'application/vnd.openxmlformats-officedocument.presentationml.presentation',
//         body: fs.createReadStream(filePath),
//       },
//     });
//     return response.data;
//   } catch (error) {
//     console.log(error.message);
//     throw new Error('Failed to upload file to Google Drive');
//   }
// }

// module.exports = { uploadFile };
