import React, { useState } from 'react';
import { X, User, Mail, Lock, CreditCard } from 'lucide-react';
import api from '../services/api';

interface AddCustomerModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

const AddCustomerModal: React.FC<AddCustomerModalProps> = ({ isOpen, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    tckn: '',
    email: '',
    password: ''
  });
  const [formLoading, setFormLoading] = useState(false);

  if (!isOpen) return null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormLoading(true);
    try {
      // Backend değişikliği yapmadan mevcut kayıt olma endpoint'ini kullanıyoruz
      await api.post('/Auth/register', formData);
      setFormData({ firstName: '', lastName: '', tckn: '', email: '', password: '' });
      onSuccess();
      onClose();
    } catch (error: any) {
      console.error('Müşteri eklenemedi:', error);
      alert(error.response?.data?.message || 'Müşteri eklenirken bir hata oluştu.');
    } finally {
      setFormLoading(false);
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content animate-fade-in" style={{maxWidth: '500px'}}>
        <div className="modal-header">
          <h2>Yeni Müşteri Kaydı</h2>
          <button className="close-btn" onClick={onClose}><X size={24} /></button>
        </div>
        
        <form onSubmit={handleSubmit} className="modal-form">
          <div style={{display: 'flex', gap: '16px'}}>
            <div className="input-group" style={{flex: 1}}>
              <label>Ad</label>
              <input 
                type="text" 
                value={formData.firstName} 
                onChange={(e) => setFormData({...formData, firstName: e.target.value})}
                placeholder="Müşteri Adı" 
                required 
              />
            </div>
            <div className="input-group" style={{flex: 1}}>
              <label>Soyad</label>
              <input 
                type="text" 
                value={formData.lastName} 
                onChange={(e) => setFormData({...formData, lastName: e.target.value})}
                placeholder="Soyadı" 
                required 
              />
            </div>
          </div>

          <div className="input-group">
            <label>TCKN</label>
            <input 
              type="text" 
              value={formData.tckn} 
              onChange={(e) => setFormData({...formData, tckn: e.target.value})}
              placeholder="11 haneli TCKN" 
              maxLength={11}
              required 
            />
          </div>

          <div className="input-group">
            <label>E-posta</label>
            <input 
              type="email" 
              value={formData.email} 
              onChange={(e) => setFormData({...formData, email: e.target.value})}
              placeholder="email@example.com" 
              required 
            />
          </div>

          <div className="input-group">
            <label>Şifre</label>
            <input 
              type="password" 
              value={formData.password} 
              onChange={(e) => setFormData({...formData, password: e.target.value})}
              placeholder="Müşteri şifresi" 
              required 
            />
          </div>

          <div className="modal-actions">
            <button type="button" className="cancel-btn" onClick={onClose}>Vazgeç</button>
            <button type="submit" className="submit-btn" disabled={formLoading}>
              {formLoading ? 'Kaydediliyor...' : 'Müşteriyi Oluştur'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddCustomerModal;
