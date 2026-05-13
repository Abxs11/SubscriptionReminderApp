import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<Login />} />
        
        {/* Mock pages for now */}
        <Route path="/dashboard" element={<div style={{padding: '40px'}}><h1>Customer Dashboard</h1><p>Geliştirme aşamasında...</p></div>} />
        <Route path="/admin" element={<div style={{padding: '40px'}}><h1>Admin Panel</h1><p>Geliştirme aşamasında...</p></div>} />
        
        <Route path="/" element={<Navigate to="/login" replace />} />
      </Routes>
    </Router>
  );
}

export default App;
