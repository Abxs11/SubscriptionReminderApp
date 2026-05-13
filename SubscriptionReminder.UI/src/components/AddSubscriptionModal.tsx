import React, { useState } from 'react';
import { X, Zap, Droplets, Flame, Wifi, Smartphone } from 'lucide-react';
import api from '../services/api';

interface AddSubscriptionModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  customerId: number;
}

const AddSubscriptionModal: React.FC<AddSubscriptionModalProps> = ({ isOpen, onClose, onSuccess, customerId }) => {
  const [type, setType] = useState('Electricity');
  const [providerName, setProviderName] = useState('');
  const [subscriberNumber, setSubscriberNumber] = useState('');
  const [formLoading, setFormLoading] = useState(false);

  if (!isOpen) return null;

  const handleAddSubscription = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormLoading(true);
    try {
      await api.post('/Subscriptions', {
        customerId,
        type,
        providerName,
        subscriberNumber
      });
      setProviderName('');
      setSubscriberNumber('');
      onSuccess();
      onClose();
    } catch (error) {
      console.error('Abonelik eklenemedi:', error);
      alert('Abonelik eklenirken bir hata oluştu.');
    } finally {
      setFormLoading(false);
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content animate-fade-in">
        <div className="modal-header">
          <h2>Yeni Abonelik Ekle</h2>
          <button className="close-btn" onClick={onClose}><X size={24} /></button>
        </div>
        
        <form onSubmit={handleAddSubscription} className="modal-form">
          <div className="input-group">
            <label>Abonelik Türü</label>
            <div className="type-selector">
              <div className={`type-option ${type === 'Electricity' ? 'selected' : ''}`} onClick={() => setType('Electricity')}>
                <Zap size={20} /> Elektrik
              </div>
              <div className={`type-option ${type === 'Water' ? 'selected' : ''}`} onClick={() => setType('Water')}>
                <Droplets size={20} /> Su
              </div>
              <div className={`type-option ${type === 'NaturalGas' ? 'selected' : ''}`} onClick={() => setType('NaturalGas')}>
                <Flame size={20} /> Doğalgaz
              </div>
              <div className={`type-option ${type === 'Internet' ? 'selected' : ''}`} onClick={() => setType('Internet')}>
                <Wifi size={20} /> İnternet
              </div>
              <div className={`type-option ${type === 'Gsm' ? 'selected' : ''}`} onClick={() => setType('Gsm')}>
                <Smartphone size={20} /> GSM
              </div>
            </div>
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

          <div className="modal-actions">
            <button type="button" className="cancel-btn" onClick={onClose}>Vazgeç</button>
            <button type="submit" className="submit-btn" disabled={formLoading}>
              {formLoading ? 'Ekleniyor...' : 'Aboneliği Ekle'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddSubscriptionModal;
