import React, { useState, useEffect } from 'react';
import axios from 'axios';
import 'bootstrap/dist/css/bootstrap.min.css';

// Assuming you have a TranslationJob class or interface
class TranslationJob {
  constructor(id, customerName, status, originalContent, translatedContent, price, translatorId) {
    this.id = id;
    this.customerName = customerName;
    this.status = status;
    this.originalContent = originalContent;
    this.translatedContent = translatedContent;
    this.price = price;
    this.translatorId = translatorId;
  }
}

const JobsTable = () => {
  const [jobs, setJobs] = useState([]);
  const [translators, setTranslators] = useState([]);
  const [isNewJobFormVisible, setIsNewJobFormVisible] = useState(false);
  const [newJob, setNewJob] = useState(new TranslationJob(0, '', '', '', '', 0, 0));

  useEffect(() => {
    axios.all([
      axios.get('http://localhost:7729/api/jobs/GetJobs'),
      axios.get('http://localhost:7729/api/translators/GetTranslators')
    ])
      .then(axios.spread((jobsResponse, translatorsResponse) => {
        setJobs(jobsResponse.data);
        setTranslators(translatorsResponse.data);
      }))
      .catch(error => console.error('Error fetching data:', error));
  }, []);

  const handleAddNewJob = () => {
    setIsNewJobFormVisible(true);
  };

  const handleSaveJob = () => {
    axios.post('http://localhost:7729/api/jobs/CreateJob', newJob)
      .then(response => {
        console.log('Job saved successfully:', response.data);
        setIsNewJobFormVisible(false);
        setNewJob(new TranslationJob(0, '', '', '', '', 0, 0));
  
        // Fetch the updated job list after saving the new job
        axios.get('http://localhost:7729/api/jobs/GetJobs')
          .then(response => setJobs(response.data))
          .catch(error => console.error('Error fetching jobs:', error));
      })
      .catch(error => console.error('Error saving job:', error));
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setNewJob(prevJob => ({
      ...prevJob,
      [name]: value,
    }));
  };

  const handleDropdownChange = (e, jobId) => {
    const { value } = e.target;
    setJobs(prevJobs =>
      prevJobs.map(job =>
        job.id === jobId ? { ...job, translatorId: value } : job
      )
    );
  };

  const handleStatusChange = (jobId, currentStatus, translatorId) => {
    let newStatus = '';
    if (currentStatus === 'New') {
      newStatus = 'In Progress';
    } else if (currentStatus === 'In Progress') {
      newStatus = 'Completed';
    }

    axios.post(`http://localhost:7729/api/jobs/UpdateJobStatus?jobId=${jobId}&translatorId=${translatorId}&newStatus=${newStatus}`)
      .then(response => {
        console.log('Job status updated successfully:', response.data);
        setJobs(prevJobs =>
          prevJobs.map(job =>
            job.id === jobId ? { ...job, status: newStatus } : job
          )
        );
      })
      .catch(error => console.error('Error updating job status:', error));
  };

  return (
    <div className="container-fluid">
      <div className="row">
        <div className="col-md-8">
          <h2>Jobs Table</h2>
          <table className="table table-bordered">
            <thead>
              <tr>
                <th>ID</th>
                <th>Customer Name</th>
                <th>Status</th>
                <th>Original Content</th>
                <th>Translated Content</th>
                <th>Price</th>
                <th>Translator</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {jobs.map((job, index) => (
                <tr key={index}>
                  <td>{index + 1}</td>
                  <td>{job.customerName}</td>
                  <td>{job.status}</td>
                  <td>{job.originalContent}</td>
                  <td>{job.translatedContent}</td>
                  <td>{job.price}</td>
                  <td>
                    <select
                      value={job.translatorId}
                      onChange={(e) => handleDropdownChange(e, job.id)}
                    >
                      <option value="">Select Translator</option>
                      {translators.map(translator => (
                        <option key={translator.id} value={translator.id}>
                          {translator.name}
                        </option>
                      ))}
                    </select>
                  </td>
                  <td>
                    <button
                      className="btn btn-success"
                      onClick={() => handleStatusChange(job.id, job.status, job.translatorId)}
                    >
                      Change Status
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {!isNewJobFormVisible && (
            <button onClick={handleAddNewJob} className="btn btn-primary">
              Add new
            </button>
          )}

          {isNewJobFormVisible && (
            <div className="mb-4">
              <label>Customer Name:</label>
              <input
                type="text"
                name="customerName"
                value={newJob.customerName}
                onChange={handleChange}
                className="form-control"
              />
              <label className="mt-2">Original Content:</label>
              <input
                type="text"
                name="originalContent"
                value={newJob.originalContent}
                onChange={handleChange}
                className="form-control"
              />
              <button onClick={handleSaveJob} className="btn btn-success mt-2">
                Save
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default JobsTable;
