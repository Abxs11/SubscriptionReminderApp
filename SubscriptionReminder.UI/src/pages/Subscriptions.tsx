import React, { useEffect, useState } from 'react';
import api from '../services/api';
import './Pages.css';
import { 
  Plus, 
  Trash2, 
  Zap, 
  Droplets, 
  Wifi, 
  Smartphone, 
  Flame,
  Search
} from 'lucide-react';
import Sidebar from '../components/Sidebar';
import AddSubscriptionModal from '../components/AddSubscriptionModal';
import EditSubscriptionModal from '../components/EditSubscriptionModal';
import PeriodInquiryModal from '../components/PeriodInquiryModal';
import { Edit2, FileSearch } from 'lucide-react';

const Subscriptions: React.FC = () => {
  const [subscriptions, setSubscriptions] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isInquiryModalOpen, setIsInquiryModalOpen] = useState(false);
  const [selectedSub, setSelectedSub] = useState<any>(null);
  const user = JSON.parse(localStorage.getItem('user') || '{}');

  useEffect(() => {
    fetchSubscriptions();
  }, []);

  const fetchSubscriptions = async () => {
    try {
      const response = await api.get(`/Subscriptions/customer/${user.customerId}`);
      setSubscriptions(response.data);
    } catch (error) {
      console.error('Abonelikler yüklenemedi:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Bu aboneliği silmek istediğinize emin misiniz?')) return;
    try {
      await api.delete(`/Subscriptions/${id}`);
      setSubscriptions(subscriptions.filter(s => s.id !== id));
    } catch (error) {
      alert('Silme işlemi başarısız oldu.');
    }
  };

  const getIcon = (type: string) => {
    switch (type) {
      case 'Electricity': return <Zap size={20} />;
      case 'Water': return <Droplets size={20} />;
      case 'NaturalGas': return <Flame size={20} />;
      case 'Internet': return <Wifi size={20} />;
      case 'Gsm': return <Smartphone size={20} />;
      default: return <Plus size={20} />;
    }
  };

  const filteredSubs = subscriptions.filter(s => 
    s.providerName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    s.subscriberNumber.includes(searchTerm)
  );

  return (
    <div className="dashboard-layout">
      <Sidebar activePage="subscriptions" />
      <main className="main-content">
        <header className="content-header">
          <div className="header-search">
            <Search size={18} />
            <input 
              type="text" 
              placeholder="Abonelik veya kurum ara..." 
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </header>

        <section className="page-content animate-fade-in">
          <div className="section-header">
            <div>
              <h1>Aboneliklerim</h1>
              <p>Toplam {subscriptions.length} aktif aboneliğiniz bulunuyor.</p>
            </div>
            <button className="add-btn" onClick={() => setIsModalOpen(true)}><Plus size={18} /> Yeni Abonelik</button>
          </div>

          <div className="subs-grid">
            {filteredSubs.map(sub => (
              <div className="sub-card" key={sub.id}>
                <div className={`sub-icon type-${sub.type.toLowerCase()}`}>
                  {getIcon(sub.type)}
                </div>
                <div className="sub-info">
                  <h3>{sub.providerName}</h3>
                  <p>{sub.subscriberNumber}</p>
                  <span className={`badge type-${sub.type.toLowerCase()}`}>{sub.type}</span>
                </div>
                <div className="card-actions">
                  <button className="inquiry-action" title="Dönem Borcu Sorgula" onClick={() => { setSelectedSub(sub); setIsInquiryModalOpen(true); }}>
                    <FileSearch size={18} />
                  </button>
                  <button className="edit-action" title="Düzenle" onClick={() => { setSelectedSub(sub); setIsEditModalOpen(true); }}>
                    <Edit2 size={18} />
                  </button>
                  <button className="delete-action" title="Sil" onClick={() => handleDelete(sub.id)}>
                    <Trash2 size={18} />
                  </button>
                </div>
              </div>
            ))}
            
            {filteredSubs.length === 0 && !loading && (
              <div className="empty-state-container">
                <p>Aradığınız kriterlere uygun abonelik bulunamadı.</p>
              </div>
            )}
          </div>
        </section>
      </main>

      <AddSubscriptionModal 
        isOpen={isModalOpen} 
        onClose={() => setIsModalOpen(false)} 
        onSuccess={fetchSubscriptions} 
        customerId={user.customerId} 
      />

      <EditSubscriptionModal
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        onSuccess={fetchSubscriptions}
        subscription={selectedSub}
      />

      <PeriodInquiryModal
        isOpen={isInquiryModalOpen}
        onClose={() => setIsInquiryModalOpen(false)}
        subscription={selectedSub}
      />
    </div>
  );
};

export default Subscriptions;
