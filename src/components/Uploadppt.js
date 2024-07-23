import React, { useState, useEffect } from "react";
import axios from "axios";

function Uploadppt() {
  const [pptDetails, setPptDetails] = useState([]);
  const [pptFiles, setPptFiles] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    loadPptDetails();
  }, []);

  const loadPptDetails = async () => {
    try {
      const response = await axios.get("https://localhost:7270/api/PPTUpload/getpptnames");
      setPptDetails(response.data);
    } catch (err) {
      console.error("Error loading PPT details:", err);
    }
  };

  const handleFileChange = (event) => {
    const files = Array.from(event.target.files);
    setPptFiles(files);
  };

  // const simulateProgress = () => {
  //   let progress = 0;
  //   const interval = setInterval(() => {
  //     progress += 5;
  //     setProgress(progress);
  //     if (progress >= 100) {
  //       clearInterval(interval);
  //     }
  //   }, 100);
  // };


  const save = async (event) => {
    event.preventDefault();
    if (pptFiles.length === 0) {
      alert("Please select at least one file first");
      return;
    }
    setIsLoading(true);
    // simulateProgress();
    try {
      const formData = new FormData();
      pptFiles.forEach((file, index) => {
        formData.append(`pptFiles`, file);
      });

      const response = await axios.post("https://localhost:7270/api/PPTUpload/upload", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });

      if (response.status === 200) {
        alert("Upload successful");
        loadPptDetails();
      }
    } catch (err) {
      console.error("Error uploading files:", err);
    }
    finally {
      setIsLoading(false);
      // setProgress(0);
  };
}

  const handleShow = async (driveFileId) => {
    try {
      const response = await axios.get(`https://localhost:7270/api/PPTUpload/getDriveFilePath/${driveFileId}`);
      if (response.status === 200) {
        window.open(response.data.driveFilePath, '_blank');
      }
    } catch (err) {
      console.error("Error fetching Drive file path:", err);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm("Are you sure you want to delete this file?")) {
      try {
        const response = await axios.delete(`https://localhost:7270/api/PPTUpload/delete/${id}`);
        debugger
        if (response.status === 200) {
          alert("File deleted successfully");
          loadPptDetails();
        }
      } catch (err) {
        console.error("Error deleting file:", err);
      }
    }
  };

  return (
    <div className="control d-flex justify-content-center align-items-center vh-100 bg-primary">

      <div className="row">
        {/* Upload Section */}
        <div className="col-md-12">
          <div className="card p-3">
            <h2 className="text-center">Upload PPT Drive Files</h2>
            {isLoading ? (
              <div className="text-center">
                <div className="spinner-border" role="status">
                  <span className="sr-only">Loading...</span>
                </div>
              </div>
            ) : (
              <>
                <div className="form-group mt-4">
                  <input type="file" multiple className="form-control-file" onChange={handleFileChange} />
                </div>
                <button className="btn btn-primary mt-4" onClick={save}>Upload</button>
              </>
            )}
          </div>
        </div>

        {/* Uploaded Files Section */}
        <div className="col-md-12 mt-3">
          <div className="card p-3">
            <h2 className="text-center">Uploaded Files</h2>
            {pptDetails.length === 0 ? (
              <div className="text-center text-danger fs-3">
              <p>No files found</p>
          </div>
          
            ) : (
            <div className="row">
              {pptDetails.map((ppt, index) => (
                <div className="col-md-4 mb-3" key={index}>
                  <div className="card">
                    <img
                      src={`https://docs.google.com/thumbnail?id=${ppt.driveFileId}`} // Google Drive thumbnail URL
                      className="card-img-top"
                      alt={ppt.pptFileName}
                    />
                    <div className="card-body">
                      <h5 className="card-title">{ppt.pptFileName}</h5>
                      <button
                        onClick={() => handleShow(ppt.driveFileId)}
                        className="btn btn-info btn-sm btn-block"
                      >
                        Show
                      </button>
                      <button
                    onClick={() => handleDelete(ppt.id)}
                    className="toggleBtn1 btn btn-danger btn-sm btn-block"
                    
                  >
                    Delete
                  </button>
                    </div>
                  </div>
                </div>
              ))}
            </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}


export default Uploadppt;
