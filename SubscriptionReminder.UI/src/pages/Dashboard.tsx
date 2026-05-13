import React, { useEffect, useState } from 'react';
import api from '../services/api';
import './Dashboard.css';
import {
  CreditCard,
  AlertCircle,
  CheckCircle2,
  TrendingUp,
  Plus,
  Search,
  X,
  Zap,
  Droplets,
  Wifi,
  Smartphone,
  Flame
} from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import Sidebar from '../components/Sidebar';

import AddSubscriptionModal from '../components/AddSubscriptionModal';

interface SummaryData {
  totalSubscriptions: number;
  activeSubscriptions: number;
  unpaidSubscriptionsThisMonth: number;
  totalPaidThisMonth: number;
  recentSubscriptions: any[];
  recentPayments: any[];
}

const Dashboard: React.FC = () => {
  const [summary, setSummary] = useState<SummaryData | null>(null);
  const [loading, setLoading] = useState(true);
  const [user, setUser] = useState<any>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const navigate = useNavigate();

  useEffect(() => {
    const userData = localStorage.getItem('user');
    if (userData) {
      setUser(JSON.parse(userData));
    } else {
      navigate('/login');
    }

    fetchSummary();
  }, [navigate]);

  const fetchSummary = async () => {
    try {
      const response = await api.get('/Summaries/dashboard');
      setSummary(response.data);
    } catch (error) {
      console.error('Özet yüklenemedi:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div className="loading-screen"><div className="loader large"></div></div>;

  return (
    <div className="dashboard-layout">
      <Sidebar activePage="dashboard" />

      {/* Main Content */}
      <main className="main-content">
        <header className="content-header">
          <div className="header-search">
            <Search size={18} />
            <input type="text" placeholder="Abonelik ara..." />
          </div>
          <div className="user-profile">
            <div className="user-info">
              <span className="user-name">{user?.email.split('@')[0]}</span>
              <span className="user-role">{user?.role}</span>
            </div>
            <div className="avatar">
              {user?.email[0].toUpperCase()}
            </div>
          </div>
        </header>

        <section className="dashboard-content">
          <div className="welcome-banner">
            <h1>Hoş geldin, {user?.email.split('@')[0]}! 👋</h1>
            <p>Bugün ödenmesi gereken <span>{summary?.unpaidSubscriptionsThisMonth}</span> aboneliğin var.</p>
          </div>

          <div className="stats-grid">
            <div className="stat-card">
              <div className="stat-icon purple"><CreditCard size={24} /></div>
              <div className="stat-details">
                <h3>Toplam Abonelik</h3>
                <p className="stat-value">{summary?.totalSubscriptions}</p>
              </div>
            </div>
            <div className="stat-card">
              <div className="stat-icon green"><CheckCircle2 size={24} /></div>
              <div className="stat-details">
                <h3>Aktif Olanlar</h3>
                <p className="stat-value">{summary?.activeSubscriptions}</p>
              </div>
            </div>
            <div className="stat-card">
              <div className="stat-icon orange"><AlertCircle size={24} /></div>
              <div className="stat-details">
                <h3>Ödenmemiş</h3>
                <p className="stat-value highlight">{summary?.unpaidSubscriptionsThisMonth}</p>
              </div>
            </div>
            <div className="stat-card">
              <div className="stat-icon emerald"><TrendingUp size={24} /></div>
              <div className="stat-details">
                <h3>Aylık Toplam</h3>
                <p className="stat-value">₺{summary?.totalPaidThisMonth.toLocaleString('tr-TR')}</p>
              </div>
            </div>
          </div>

          <div className="data-grid">
            <div className="data-section">
              <div className="section-header">
                <h2>Son Abonelikler</h2>
                <button className="add-btn" onClick={() => setIsModalOpen(true)}><Plus size={18} /> Yeni Ekle</button>
              </div>
              <div className="data-table-wrapper">
                <table className="data-table">
                  <thead>
                    <tr>
                      <th>Tür</th>
                      <th>Sağlayıcı</th>
                      <th>Abone No</th>
                      <th>Durum</th>
                    </tr>
                  </thead>
                  <tbody>
                    {summary?.recentSubscriptions.map((sub: any) => (
                      <tr key={sub.id}>
                        <td><span className={`badge type-${sub.type.toLowerCase()}`}>{sub.type}</span></td>
                        <td>{sub.providerName}</td>
                        <td>{sub.subscriberNumber}</td>
                        <td><span className={`status-dot ${sub.status.toLowerCase()}`}></span> {sub.status}</td>
                      </tr>
                    ))}
                    {summary?.recentSubscriptions.length === 0 && (
                      <tr><td colSpan={4} className="empty-state">Henüz abonelik bulunmuyor.</td></tr>
                    )}
                  </tbody>
                </table>
              </div>
            </div>

            <div className="data-section">
              <div className="section-header">
                <h2>Son Ödemeler</h2>
              </div>
              <div className="data-table-wrapper">
                <table className="data-table">
                  <thead>
                    <tr>
                      <th>Dönem</th>
                      <th>Tutar</th>
                      <th>Tarih</th>
                      <th>Durum</th>
                    </tr>
                  </thead>
                  <tbody>
                    {summary?.recentPayments.map((pay: any) => (
                      <tr key={pay.id}>
                        <td>{pay.period}</td>
                        <td>₺{pay.amount}</td>
                        <td>{new Date(pay.paymentDateUtc).toLocaleDateString('tr-TR')}</td>
                        <td>
                          <span className={`payment-status ${pay.status.toLowerCase()}`}>
                            {pay.status === 'Success' ? 'Başarılı' : 'Hatalı'}
                          </span>
                        </td>
                      </tr>
                    ))}
                    {summary?.recentPayments.length === 0 && (
                      <tr><td colSpan={4} className="empty-state">Henüz ödeme bulunmuyor.</td></tr>
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </section>
      </main>

      <AddSubscriptionModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSuccess={fetchSummary}
        customerId={user?.customerId}
      />
    </div>
  );
};

export default Dashboard;
