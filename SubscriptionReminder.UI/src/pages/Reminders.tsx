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
  const [unpaid, setUnpaid] = useState<any[]>([]);
  const [debts, setDebts] = useState<{[key: number]: number}>({});
  const [loading, setLoading] = useState(true);
  const [payingId, setPayingId] = useState<number | null>(null);

  useEffect(() => {
    fetchUnpaid();
  }, []);

  const fetchUnpaid = async () => {
    setLoading(true);
    try {
      const response = await api.get('/Summaries/unpaid');
      const data = response.data;
      setUnpaid(data);
      
      // Her bir abonelik için borç sorgula
      const debtMap: {[key: number]: number} = {};
      for (const sub of data) {
        try {
          const inquiryRes = await api.post(`/DebtInquiries/${sub.id}/query`);
          debtMap[sub.id] = inquiryRes.data.amount;
        } catch (err) {
          console.error(`Borç sorgulanamadı (ID: ${sub.id}):`, err);
          debtMap[sub.id] = 0;
        }
      }
      setDebts(debtMap);
    } catch (error) {
      console.error('Ödenmemiş abonelikler yüklenemedi:', error);
    } finally {
      setLoading(false);
    }
  };

  const handlePay = async (sub: any) => {
    const amount = debts[sub.id];
    if (!amount || amount <= 0) {
      alert('Bu abonelik için ödenecek borç bulunamadı.');
      return;
    }

    setPayingId(sub.id);
    const currentPeriod = new Date().toISOString().slice(0, 7); // yyyy-MM
    
    try {
      // Ödeme yap (Mock)
      await api.post('/Payments', {
        subscriptionId: sub.id,
        amount: amount,
        period: currentPeriod
      });

      alert(`₺${amount} tutarındaki ödeme başarıyla gerçekleştirildi!`);
      fetchUnpaid(); // Listeyi yenile
    } catch (error) {
      alert('Ödeme sırasında bir hata oluştu.');
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
            <p>Bu ay henüz ödemesi yapılmamış {unpaid.length} aboneliğiniz bulunuyor.</p>
          </div>

          <div className="reminders-list">
            {unpaid.map(sub => (
              <div className="reminder-item" key={sub.id}>
                <div className="reminder-main">
                  <div className={`reminder-icon type-${sub.type.toLowerCase()}`}>
                    <Calendar size={24} />
                  </div>
                  <div className="reminder-info">
                    <h3>{sub.providerName}</h3>
                    <p>{sub.subscriberNumber} • {sub.type}</p>
                    <div className="debt-badge">
                      Borç: <span>₺{debts[sub.id] !== undefined ? debts[sub.id].toLocaleString('tr-TR') : '...'}</span>
                    </div>
                  </div>
                </div>
                
                <div className="reminder-status">
                  <div className="status-tag unpaid">
                    <AlertCircle size={16} /> Ödenmedi
                  </div>
                  <button 
                    className="pay-btn" 
                    onClick={() => handlePay(sub)}
                    disabled={payingId === sub.id || debts[sub.id] === undefined}
                  >
                    {payingId === sub.id ? 'Ödeniyor...' : (
                      <>
                        <CreditCard size={18} /> Öde (₺{debts[sub.id]?.toLocaleString('tr-TR')})
                      </>
                    )}
                  </button>
                </div>
              </div>
            ))}

            {unpaid.length === 0 && !loading && (
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
