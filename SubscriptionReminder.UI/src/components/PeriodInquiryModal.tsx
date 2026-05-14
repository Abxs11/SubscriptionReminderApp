import React, { useState, useEffect } from 'react';
import { X, Search, Calendar, AlertCircle, CheckCircle2, CreditCard } from 'lucide-react';
import api from '../services/api';

interface PeriodInquiryModalProps {
  isOpen: boolean;
  onClose: () => void;
  subscription: any;
}

const PeriodInquiryModal: React.FC<PeriodInquiryModalProps> = ({ isOpen, onClose, subscription }) => {
  const [periods, setPeriods] = useState<string[]>([]);
  const [selectedPeriod, setSelectedPeriod] = useState<string>('');
  const [status, setStatus] = useState<any>(null);
  const [loading, setLoading] = useState(false);
  const [isPaying, setIsPaying] = useState(false);

  useEffect(() => {
    if (subscription) {
      generatePeriods();
    }
  }, [subscription]);

  const generatePeriods = () => {
    if (!subscription) return;
    
    const start = new Date(subscription.createdAtUtc);
    const end = new Date();
    const months: string[] = [];
    
    let current = new Date(start.getFullYear(), start.getMonth(), 1);
    while (current <= end) {
      const year = current.getFullYear();
      const month = String(current.getMonth() + 1).padStart(2, '0');
      months.push(`${year}-${month}`);
      current.setMonth(current.getMonth() + 1);
    }
    
    setPeriods(months.reverse()); // En yeni en başta
    setSelectedPeriod(months[0]);
  };

  const handleQuery = async () => {
    if (!selectedPeriod) return;
    setLoading(true);
    setStatus(null);
    try {
      const response = await api.get(`/DebtInquiries/${subscription.id}/status/${selectedPeriod}`);
      setStatus(response.data);
    } catch (error) {
      console.error('Sorgulama hatası:', error);
      alert('Sorgulama sırasında bir hata oluştu.');
    } finally {
      setLoading(false);
    }
  };

  const handlePay = async () => {
    if (!status) return;
    setIsPaying(true);
    try {
      // Kart kontrolü (PCI-DSS senaryosu)
      const cardsResponse = await api.get('/SavedCards');
      if (cardsResponse.data.length === 0) {
        alert('Sistemde kayıtlı ödeme yönteminiz bulunmuyor. Lütfen önce "Kayıtlı Kartlarım" sayfasından bir kart ekleyin.');
        setIsPaying(false);
        return;
      }

      await api.post('/Payments', {
        subscriptionId: subscription.id,
        amount: status.amount,
        period: status.period,
        dueDate: status.dueDate
      });
      alert('Ödeme başarıyla gerçekleştirildi!');
      handleQuery(); // Durumu güncelle
    } catch (error: any) {
      const msg = error.response?.data?.message || 'Ödeme sırasında bir hata oluştu.';
      alert(msg);
    } finally {
      setIsPaying(false);
    }
  };

  if (!isOpen || !subscription) return null;

  return (
    <div className="modal-overlay">
      <div className="modal-content period-inquiry-modal">
        <div className="modal-header">
          <h2>Dönem Borcu Sorgula</h2>
          <button className="close-btn" onClick={onClose}><X size={24} /></button>
        </div>
        
        <div className="modal-body">
          <div className="sub-detail-header">
            <h3>{subscription.providerName}</h3>
            <p>{subscription.subscriberNumber} • {subscription.type}</p>
          </div>

          <div className="form-group">
            <label>Sorgulanacak Dönemi Seçin</label>
            <div className="period-selector-row">
              <select 
                value={selectedPeriod} 
                onChange={(e) => setSelectedPeriod(e.target.value)}
                className="period-select"
              >
                {periods.map(p => (
                  <option key={p} value={p}>{p}</option>
                ))}
              </select>
              <button 
                className="query-btn" 
                onClick={handleQuery}
                disabled={loading}
              >
                {loading ? 'Sorgulanıyor...' : <><Search size={18} /> Sorgula</>}
              </button>
            </div>
          </div>

          {status && (
            <div className={`status-result-card ${status.isPaid ? 'paid' : 'unpaid'}`}>
              <div className="status-icon">
                {status.isPaid ? <CheckCircle2 size={40} /> : <AlertCircle size={40} />}
              </div>
              <div className="status-info">
                <h4>{status.period} Dönemi</h4>
                {status.isPaid ? (
                  <>
                    <p className="status-msg">Bu ay dönem borcunuz <strong>₺{status.amount.toLocaleString('tr-TR')}</strong> tutarındaydı ve ödediniz.</p>
                    <span className="payment-date">Ödeme Tarihi: {new Date(status.paymentDate).toLocaleDateString('tr-TR')}</span>
                  </>
                ) : (
                  <>
                    <p className="status-msg">Bu ay için <strong>₺{status.amount.toLocaleString('tr-TR')}</strong> tutarında borcunuz bulunuyor.</p>
                    <p className="due-date">Son Ödeme: {new Date(status.dueDate).toLocaleDateString('tr-TR')}</p>
                    <button className="pay-now-btn" onClick={handlePay} disabled={isPaying}>
                      {isPaying ? 'Ödeniyor...' : <><CreditCard size={18} /> Hemen Öde</>}
                    </button>
                  </>
                )}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default PeriodInquiryModal;
