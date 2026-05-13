import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import './Login.css';
import { LogIn, Mail, Lock, ShieldCheck } from 'lucide-react';

const Login: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const response = await api.post('/Auth/login', { email, password });
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('user', JSON.stringify(response.data));
      
      // Redirect based on role
      if (response.data.role === 'Admin') {
        navigate('/admin');
      } else {
        navigate('/dashboard');
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Giriş başarısız. Lütfen bilgilerinizi kontrol edin.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <div className="login-visual">
        <div className="visual-content">
          <h1>Subscription<br /><span>Reminder</span></h1>
          <p>Ödemelerinizi unutmayın, kontrolü elinizde tutun.</p>
        </div>
      </div>
      
      <div className="login-form-side">
        <div className="login-card animate-fade-in">
          <div className="card-header">
            <div className="logo-icon">
              <ShieldCheck size={32} color="#6366f1" />
            </div>
            <h2>Hoş Geldiniz</h2>
            <p>Hesabınıza erişmek için bilgilerinizi girin</p>
          </div>

          <form onSubmit={handleLogin}>
            <div className="input-group">
              <label>Email Adresi</label>
              <div className="input-wrapper">
                <Mail size={20} className="icon" />
                <input 
                  type="email" 
                  value={email} 
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="admin@subscription.com"
                  required
                />
              </div>
            </div>

            <div className="input-group">
              <label>Şifre</label>
              <div className="input-wrapper">
                <Lock size={20} className="icon" />
                <input 
                  type="password" 
                  value={password} 
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder="••••••••"
                  required
                />
              </div>
            </div>

            {error && <div className="error-message">{error}</div>}

            <button type="submit" className="login-button" disabled={loading}>
              {loading ? <span className="loader"></span> : (
                <>
                  <LogIn size={20} />
                  Giriş Yap
                </>
              )}
            </button>
          </form>

          <div className="card-footer">
            <p>Hesabınız yok mu? <span className="link">Kayıt Ol</span></p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
