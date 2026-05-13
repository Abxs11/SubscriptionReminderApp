import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import api from '../services/api';
import './Login.css'; // Login ile aynı stilleri kullanabiliriz
import { User, Mail, Lock, CreditCard, ArrowRight, UserPlus } from 'lucide-react';

const Register: React.FC = () => {
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    tckn: '',
    email: '',
    password: ''
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      await api.post('/Auth/register', formData);
      alert('Kayıt başarılı! Şimdi giriş yapabilirsiniz.');
      navigate('/login');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Kayıt sırasında bir hata oluştu.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <div className="login-card animate-fade-in">
        <div className="login-header">
          <div className="logo-icon">
            <UserPlus size={32} />
          </div>
          <h1>Yeni Hesap Oluştur</h1>
          <p>Subscription & Payment Reminder</p>
        </div>

        {error && <div className="error-message">{error}</div>}

        <form onSubmit={handleRegister} className="login-form">
          <div className="input-row">
            <div className="input-group">
              <label><User size={16} /> Ad</label>
              <input
                type="text"
                name="firstName"
                placeholder="Adınız"
                value={formData.firstName}
                onChange={handleChange}
                required
              />
            </div>
            <div className="input-group">
              <label><User size={16} /> Soyad</label>
              <input
                type="text"
                name="lastName"
                placeholder="Soyadınız"
                value={formData.lastName}
                onChange={handleChange}
                required
              />
            </div>
          </div>

          <div className="input-group">
            <label><CreditCard size={16} /> TCKN</label>
            <input
              type="text"
              name="tckn"
              placeholder="11 haneli TCKN"
              maxLength={11}
              value={formData.tckn}
              onChange={handleChange}
              required
            />
          </div>

          <div className="input-group">
            <label><Mail size={16} /> E-posta</label>
            <input
              type="email"
              name="email"
              placeholder="E-posta adresiniz"
              value={formData.email}
              onChange={handleChange}
              required
            />
          </div>

          <div className="input-group">
            <label><Lock size={16} /> Şifre</label>
            <input
              type="password"
              name="password"
              placeholder="••••••••"
              value={formData.password}
              onChange={handleChange}
              required
            />
          </div>

          <button type="submit" className="login-button" disabled={loading}>
            {loading ? 'Kaydediliyor...' : (
              <>
                Kayıt Ol <ArrowRight size={20} />
              </>
            )}
          </button>
        </form>

        <div className="login-footer">
          Zaten hesabın var mı? <Link to="/login">Giriş Yap</Link>
        </div>
      </div>
    </div>
  );
};

export default Register;
