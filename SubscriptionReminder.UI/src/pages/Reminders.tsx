import React, { useEffect, useState } from 'react';
import api from '../services/api';
import './Pages.css';
import {
  Bell,
  CreditCard,
  AlertCircle,
  CheckCircle2,
  Calendar
} from 'lucide-react';
import Sidebar from '../components/Sidebar';

const Reminders: React.FC = () => {
  const [unpaidItems, setUnpaidItems] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [payingId, setPayingId] = useState<string | null>(null); // subscriptionId-period combinations

  useEffect(() => {
    fetchUnpaid();
  }, []);

  const fetchUnpaid = async () => {
    setLoading(true);
    try {
      const response = await api.get('/Summaries/unpaid');
      setUnpaidItems(response.data);
    } catch (error) {
      console.error('Ödenmemiş abonelikler yüklenemedi:', error);
    } finally {
      setLoading(false);
    }
  };

  const handlePay = async (item: any) => {
    const uniqueKey = `${item.id}-${item.period}`;
    setPayingId(uniqueKey);
    
    try {
      // Ödeme yap (Mock)
      await api.post('/Payments', {
        subscriptionId: item.id,
        amount: item.amount,
        period: item.period,
        dueDate: item.dueDate
      });

      alert(`${item.period} dönemine ait ₺${item.amount} tutarındaki ödeme başarıyla gerçekleştirildi!`);
      fetchUnpaid(); // Listeyi yenile
    } catch (error: any) {
      const msg = error.response?.data?.message || 'Ödeme sırasında bir hata oluştu.';
      alert(msg);
    } finally {
      setPayingId(null);
    }
  };

  return (
    <div className="dashboard-layout">
      <Sidebar activePage="reminders" />
      <main className="main-content">
        <header className="content-header">
          <div className="page-title">
            <Bell size={20} color="#8b5cf6" />
            <span>Ödeme Hatırlatıcıları</span>
          </div>
        </header>

        <section className="page-content animate-fade-in">
          <div className="welcome-banner">
            <h1>Ödeme Zamanı! 💸</h1>
            <p>Şu an ödenmesi gereken toplam {unpaidItems.length} borç kaleminiz bulunuyor.</p>
          </div>
 
          <div className="reminders-list">
            {unpaidItems.map(item => {
              const uniqueKey = `${item.id}-${item.period}`;
              return (
                <div className="reminder-item" key={uniqueKey}>
                  <div className="reminder-main">
                    <div className={`reminder-icon type-${item.type.toLowerCase()}`}>
                      <Calendar size={24} />
                    </div>
                    <div className="reminder-info">
                      <h3>{item.providerName}</h3>
                      <p>{item.subscriberNumber} • {item.type}</p>
                      <div className="debt-info-row">
                        <div className="debt-period-tag">
                          {item.period} Dönemi
                        </div>
                        <div className="debt-badge">
                          Borç: <span>₺{item.amount.toLocaleString('tr-TR')}</span>
                        </div>
                        <div className="due-date-info">
                          <Calendar size={14} />
                          <span>Son Ödeme: {new Date(item.dueDate).toLocaleDateString('tr-TR')}</span>
                        </div>
                      </div>
                    </div>
                  </div>
                  
                  <div className="reminder-status">
                    <div className="status-tag unpaid">
                      <AlertCircle size={16} /> Ödenmedi
                    </div>
                    <button 
                      className="pay-btn" 
                      onClick={() => handlePay(item)}
                      disabled={payingId === uniqueKey}
                    >
                      {payingId === uniqueKey ? 'Ödeniyor...' : (
                        <>
                          <CreditCard size={18} /> Öde (₺{item.amount.toLocaleString('tr-TR')})
                        </>
                      )}
                    </button>
                  </div>
                </div>
              );
            })}
 
            {unpaidItems.length === 0 && !loading && (
              <div className="all-paid-state">
                <CheckCircle2 size={64} color="#10b981" />
                <h2>Harika! Tüm ödemeler tamam.</h2>
                <p>Bu ay için ödenmemiş herhangi bir aboneliğiniz kalmadı.</p>
              </div>
            )}
          </div>
        </section>
      </main>
    </div>
  );
};

export default Reminders;
