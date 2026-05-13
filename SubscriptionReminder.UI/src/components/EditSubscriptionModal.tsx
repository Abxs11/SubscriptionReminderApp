import React, { useState, useEffect } from 'react';
import { X, Zap, Droplets, Flame, Wifi, Smartphone } from 'lucide-react';
import api from '../services/api';

interface EditSubscriptionModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  subscription: any;
}

const EditSubscriptionModal: React.FC<EditSubscriptionModalProps> = ({ isOpen, onClose, onSuccess, subscription }) => {
  const [providerName, setProviderName] = useState('');
  const [subscriberNumber, setSubscriberNumber] = useState('');
  const [status, setStatus] = useState('Active');
  const [formLoading, setFormLoading] = useState(false);

  useEffect(() => {
    if (subscription) {
      setProviderName(subscription.providerName);
      setSubscriberNumber(subscription.subscriberNumber);
      setStatus(subscription.status);
    }
  }, [subscription]);

  if (!isOpen || !subscription) return null;

  const handleUpdateSubscription = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormLoading(true);
    try {
      await api.put(`/Subscriptions/${subscription.id}`, {
        providerName,
        subscriberNumber,
        status
      });
      onSuccess();
      onClose();
    } catch (error) {
      console.error('Abonelik güncellenemedi:', error);
      alert('Güncelleme sırasında bir hata oluştu.');
    } finally {
      setFormLoading(false);
    }
  };

  const getIcon = (type: string) => {
    switch (type) {
      case 'Electricity': return <Zap size={20} />;
      case 'Water': return <Droplets size={20} />;
      case 'NaturalGas': return <Flame size={20} />;
      case 'Internet': return <Wifi size={20} />;
      case 'Gsm': return <Smartphone size={20} />;
      default: return <Zap size={20} />;
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content animate-fade-in">
        <div className="modal-header">
          <div className="header-title">
            <span className={`icon-box type-${subscription.type.toLowerCase()}`}>
              {getIcon(subscription.type)}
            </span>
            <h2>Aboneliği Düzenle</h2>
          </div>
          <button className="close-btn" onClick={onClose}><X size={24} /></button>
        </div>
        
        <form onSubmit={handleUpdateSubscription} className="modal-form">
          <div className="input-group">
            <label>Abonelik Türü</label>
            <input type="text" value={subscription.type} disabled className="disabled-input" />
            <small>Tür değiştirilemez, gerekirse silip tekrar ekleyiniz.</small>
          </div>

          <div className="input-group">
            <label>Kurum / Sağlayıcı Adı</label>
            <input 
              type="text" 
              value={providerName} 
              onChange={(e) => setProviderName(e.target.value)}
              placeholder="Örn: CK Boğaziçi, Türk Telekom..." 
              required 
            />
          </div>

          <div className="input-group">
            <label>Abone Numarası</label>
            <input 
              type="text" 
              value={subscriberNumber} 
              onChange={(e) => setSubscriberNumber(e.target.value)}
              placeholder="Abone veya Sözleşme No" 
              required 
            />
          </div>

          <div className="input-group">
            <label>Durum</label>
            <select value={status} onChange={(e) => setStatus(e.target.value)} className="status-select">
              <option value="Active">Aktif</option>
              <option value="Passive">Pasif</option>
            </select>
          </div>

          <div className="modal-actions">
            <button type="button" className="cancel-btn" onClick={onClose}>Vazgeç</button>
            <button type="submit" className="submit-btn" disabled={formLoading}>
              {formLoading ? 'Güncelleniyor...' : 'Değişiklikleri Kaydet'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EditSubscriptionModal;
