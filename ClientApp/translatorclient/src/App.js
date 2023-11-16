import './App.css';
import JobsTable from './JobsTable'; 


function App() {
  return (
    <div className="App">
      <header className="App-header">
        <p>
          Hello there, here is a list of all the jobs :
        </p>
        <div className='jobs-table'>
          <JobsTable />
        </div>
      </header>
    </div>
  );
}

export default App;
