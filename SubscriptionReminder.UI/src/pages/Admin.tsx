import React, { useEffect, useState } from 'react';
import api from '../services/api';
import './Pages.css';
import {
  Users,
  UserPlus,
  Trash2,
  Search,
  ExternalLink,
  Shield,
  Mail,
  UserCheck,
  TrendingUp
} from 'lucide-react';
import Sidebar from '../components/Sidebar';

import AddCustomerModal from '../components/AddCustomerModal';

const Admin: React.FC = () => {
  const [customers, setCustomers] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);

  useEffect(() => {
    fetchCustomers();
  }, []);

  const fetchCustomers = async () => {
    try {
      const response = await api.get('/Customers');
      setCustomers(response.data);
    } catch (error) {
      console.error('Müşteriler yüklenemedi:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Bu müşteriyi ve tüm aboneliklerini silmek istediğinize emin misiniz?')) return;
    try {
      await api.delete(`/Customers/${id}`);
      setCustomers(customers.filter(c => c.id !== id));
    } catch (error) {
      alert('Silme işlemi başarısız oldu.');
    }
  };

  const filteredCustomers = customers.filter(c =>
    c.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    c.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    c.email.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="dashboard-layout">
      <Sidebar activePage="admin" />
      <main className="main-content">
        <header className="content-header-premium">
          <div className="page-title">
            <Shield size={24} color="#8b5cf6" />
            <h1>Sistem Yönetim Merkezi</h1>
          </div>
        </header>

        <section className="page-content animate-fade-in">
          <div className="admin-welcome-banner">
            <div className="banner-text">
              <h1>Admin Paneline Hoş Geldiniz ✨</h1>
              <p>Platformdaki tüm müşteri hareketlerini ve kayıtlarını buradan yönetebilirsiniz.</p>
            </div>
            <button className="admin-add-btn" onClick={() => setIsModalOpen(true)}>
              <UserPlus size={20} /> Yeni Müşteri Oluştur
            </button>
          </div>

          <div className="admin-stats-grid">
            <div className="admin-stat-card">
              <div className="stat-icon-box purple">
                <Users size={24} />
              </div>
              <div className="stat-info">
                <div className="stat-value">{customers.length}</div>
                <div className="stat-label">Toplam Kayıtlı Müşteri</div>
              </div>
            </div>
            <div className="admin-stat-card">
              <div className="stat-icon-box emerald">
                <UserCheck size={24} />
              </div>
              <div className="stat-info">
                <div className="stat-value">{customers.filter(c => c.tckn).length}</div>
                <div className="stat-label">Doğrulanmış Hesaplar</div>
              </div>
            </div>
            <div className="admin-stat-card">
              <div className="stat-icon-box orange">
                <TrendingUp size={24} />
              </div>
              <div className="stat-info">
                <div className="stat-value">{new Date().toLocaleDateString('tr-TR', { month: 'long' })}</div>
                <div className="stat-label">Aktif İzleme Dönemi</div>
              </div>
            </div>
          </div>

          <div className="admin-data-section">
            <div className="section-header">
              <h2><Search size={20} /> Müşteri Listesi & Son Kayıtlar</h2>
              <div className="header-search-box">
                <input
                  type="text"
                  placeholder="İsim, email veya TCKN ile ara..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
              </div>
            </div>

            <div className="data-table-wrapper">
              <table className="data-table admin-table">
                <thead>
                  <tr>
                    <th>Müşteri Bilgileri</th>
                    <th>Email Adresi</th>
                    <th>TC Kimlik No</th>
                    <th>Kayıt Tarihi</th>
                    <th style={{ textAlign: 'right' }}>İşlemler</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredCustomers.map(customer => (
                    <tr key={customer.id}>
                      <td>
                        <div className="customer-cell">
                          <div className="avatar-small">{customer.firstName[0]}{customer.lastName[0]}</div>
                          <div className="name-info">
                            <span className="full-name">{customer.firstName} {customer.lastName}</span>
                            <span className="customer-id">ID: #{customer.id}</span>
                          </div>
                        </div>
                      </td>
                      <td>{customer.email}</td>
                      <td>{customer.tckn}</td>
                      <td>{new Date(customer.createdAtUtc).toLocaleDateString('tr-TR')}</td>
                      <td style={{ textAlign: 'right' }}>
                        <button className="delete-action-btn" onClick={() => handleDelete(customer.id)} title="Müşteriyi Sil">
                          <Trash2 size={18} />
                        </button>
                      </td>
                    </tr>
                  ))}
                  {filteredCustomers.length === 0 && !loading && (
                    <tr><td colSpan={5} className="empty-state">Henüz hiç müşteri kaydı bulunmuyor.</td></tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </section>
      </main>

      <AddCustomerModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSuccess={fetchCustomers}
      />
    </div>
  );
};

export default Admin;
