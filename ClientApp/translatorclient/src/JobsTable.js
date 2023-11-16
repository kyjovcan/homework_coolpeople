import React, { useState, useEffect } from 'react';
import axios from 'axios';

const JobsTable = () => {
  const [jobs, setJobs] = useState([]);

  useEffect(() => {
    axios.get('http://localhost:7729/GetJobs')
      .then(response => {
        console.log("response from backend: ")
        console.log(response.data)
        setJobs(response.data)

      })
      .catch(error => console.error('Error fetching jobs:', error));
  }, []);


  return (
    <div>
      <h1>Current Jobs</h1>
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Customer Name</th>
            <th>Status</th>
            <th>Original Content</th>
            <th>Translated Content</th>
            <th>Price</th>
          </tr>
        </thead>
        <tbody>
          {jobs.map(job => (
            <tr key={job.id}> 
              <td>{job.id}</td>
              <td>{job.customerName}</td>
              <td>{job.status}</td>
              <td>{job.originalContent}</td>
              <td>{job.translatedContent}</td>
              <td>{job.price}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default JobsTable;