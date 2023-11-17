import React, { useState, useEffect } from 'react';
import axios from 'axios';
import 'bootstrap/dist/css/bootstrap.min.css'; 

const TranslatorsTable = () => {
  const [translators, setTranslators] = useState([]);
  const [showNewTranslatorForm, setShowNewTranslatorForm] = useState(false);
  const [newTranslator, setNewTranslator] = useState({
    name: '',
    hourlyRate: '',
    status: '',
    creditCardNumber: '',
  });

  useEffect(() => {
    axios.get('http://localhost:7729/api/translators/GetTranslators')
      .then(response => setTranslators(response.data))
      .catch(error => console.error('Error fetching translators:', error));
  }, [translators]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setNewTranslator({
      ...newTranslator,
      [name]: value,
    });
  };

  const handleAddNewTranslator = () => {
    setShowNewTranslatorForm(true);
  };

  const handleSaveNewTranslator = () => {
    axios.post('http://localhost:7729/api/translators/AddTranslator', newTranslator)
      .then(response => {
        console.log('Translator added successfully:', response.data);
        setShowNewTranslatorForm(false);
      })
      .catch(error => console.error('Error adding translator:', error));
  };

  return (
    <div>
      <h2>Translators Table</h2>
      <table className="table table-bordered">
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Hourly Rate</th>
            <th>Status</th>
            {/* <th>Credit Card Number</th> */}
          </tr>
        </thead>
        <tbody>
          {translators.map((translator, index) => (
            <tr key={index}>
              <td>{index + 1}</td>
              <td>{translator.name}</td>
              <td>{translator.hourlyRate}</td>
              <td>{translator.status}</td>
              {/* <td>{translator.creditCardNumber}</td> */}
            </tr>
          ))}
        </tbody>
      </table>
      
      {showNewTranslatorForm ? (
        <div>
          <label>Name:</label>
          <input type="text" name="name" value={newTranslator.name} onChange={handleInputChange} />
          <label>Hourly Rate:</label>
          <input type="text" name="hourlyRate" value={newTranslator.hourlyRate} onChange={handleInputChange} />
          <label>Status:</label>
          <input type="text" name="status" value={newTranslator.status} onChange={handleInputChange} />
          <label>Credit Card Number:</label>
          <input type="text" name="creditCardNumber" value={newTranslator.creditCardNumber} onChange={handleInputChange} />
          <button onClick={handleSaveNewTranslator}>Save</button>
        </div>
      ) : (
        <button onClick={handleAddNewTranslator} className="btn btn-primary">Add New</button>
      )}
    </div>
  );
};

export default TranslatorsTable;