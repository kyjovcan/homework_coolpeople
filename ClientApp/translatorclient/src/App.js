import './App.css';
import JobsTable from './JobsTable';
import TranslatorsTable from './TranslatorsTable';


function App() {
  return (
    <div className="App">
      <header className="App-header">
        <div className="container-fluid">
          <div className="row">
            <div className="col-md-8">
              <JobsTable />
            </div>

            <div className="col-md-4">
              <TranslatorsTable />
            </div>
          </div>
        </div>
      </header>
    </div>
  );
}

export default App;
