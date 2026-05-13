import React, { useEffect, useState } from 'react';
import api from '../services/api';
import Sidebar from '../components/Sidebar';
import { CreditCard, Calendar, CheckCircle, AlertCircle, Search } from 'lucide-react';
import './Pages.css';

const Payments: React.FC = () => {
  const [payments, setPayments] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    fetchPayments();
  }, []);

  const fetchPayments = async () => {
    try {
      const response = await api.get('/Summaries/payments');
      setPayments(response.data);
    } catch (error) {
      console.error('Ödeme geçmişi yükleme hatası:', error);
    } finally {
      setLoading(false);
    }
  };

  const filteredPayments = payments.filter(p => 
    p.period.toLowerCase().includes(searchTerm.toLowerCase()) ||
    p.status.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('tr-TR', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <div className="dashboard-layout">
      <Sidebar activePage="payments" />
      
      <main className="main-content">
        <header className="content-header-premium">
          <h1>
            <CreditCard size={24} color="var(--primary-color)" />
            Ödeme Geçmişi
          </h1>
          <div className="header-search-box">
            <Search size={18} color="var(--text-secondary)" />
            <input 
              type="text" 
              placeholder="Dönem veya durum ara..." 
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </header>

        <section className="page-content animate-fade-in">
          <div className="admin-data-section">
            <table className="admin-table">
              <thead>
                <tr>
                  <th>Abonelik</th>
                  <th>Dönem</th>
                  <th>Tutar</th>
                  <th>Ödeme Tarihi</th>
                  <th>İşlem ID</th>
                  <th>Durum</th>
                </tr>
              </thead>
              <tbody>
                {loading ? (
                  <tr><td colSpan={6} style={{textAlign: 'center', padding: '40px'}}>Yükleniyor...</td></tr>
                ) : filteredPayments.length > 0 ? (
                  filteredPayments.map(payment => (
                    <tr key={payment.id}>
                      <td>
                        <div className="icon-text">
                          <CreditCard size={16} color="var(--primary-color)" />
                          <div style={{display: 'flex', flexDirection: 'column'}}>
                            <strong>{payment.providerName}</strong>
                            <small style={{fontSize: '0.7rem', color: 'var(--text-secondary)'}}>{payment.subscriberNumber}</small>
                          </div>
                        </div>
                      </td>
                      <td>
                        <div className="icon-text">
                          <Calendar size={16} />
                          <span>{payment.period}</span>
                        </div>
                      </td>
                      <td>
                        <span style={{fontWeight: 700, color: 'var(--text-primary)'}}>
                          ₺{payment.amount.toLocaleString('tr-TR', { minimumFractionDigits: 2 })}
                        </span>
                      </td>
                      <td style={{fontSize: '0.85rem', color: 'var(--text-secondary)'}}>
                        {formatDate(payment.paymentDateUtc)}
                      </td>
                      <td style={{fontSize: '0.75rem', fontFamily: 'monospace', color: 'var(--text-muted)'}}>
                        {payment.externalTransactionId || '-'}
                      </td>
                      <td>
                        <span className={`status-tag ${payment.status.toLowerCase() === 'success' ? 'paid' : 'unpaid'}`} style={{
                           background: payment.status.toLowerCase() === 'success' ? 'rgba(16, 185, 129, 0.1)' : 'rgba(239, 68, 68, 0.1)',
                           color: payment.status.toLowerCase() === 'success' ? '#10b981' : '#ef4444'
                        }}>
                          {payment.status.toLowerCase() === 'success' ? (
                            <><CheckCircle size={14} /> Başarılı</>
                          ) : (
                            <><AlertCircle size={14} /> Başarısız</>
                          )}
                        </span>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr><td colSpan={5} style={{textAlign: 'center', padding: '40px'}}>Henüz ödeme kaydı bulunamadı.</td></tr>
                )}
              </tbody>
            </table>
          </div>
        </section>
      </main>
    </div>
  );
};

export default Payments;
