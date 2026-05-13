import React, { useEffect, useState } from 'react';
import Sidebar from '../components/Sidebar';
import './Pages.css';
import { User as UserIcon, Mail, Shield, Calendar, Clock, BadgeCheck } from 'lucide-react';
import api from '../services/api';

const Profile: React.FC = () => {
  const [profile, setProfile] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchProfile();
  }, []);

  const fetchProfile = async () => {
    try {
      const response = await api.get('/Customers/me');
      setProfile(response.data);
    } catch (error) {
      console.error('Profil bilgileri çekilemedi:', error);
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString: string) => {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleDateString('tr-TR', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  if (loading) {
    return (
      <div className="dashboard-layout">
        <Sidebar activePage="profile" />
        <main className="main-content">
          <div className="loading-screen">Yükleniyor...</div>
        </main>
      </div>
    );
  }

  return (
    <div className="dashboard-layout">
      <Sidebar activePage="profile" />
      <main className="main-content">
        <header className="content-header-premium">
          <h1>
            <UserIcon size={24} color="var(--primary-color)" />
            Profil Bilgilerim
          </h1>
        </header>

        <section className="page-content animate-fade-in">
          <div className="profile-grid">
            {/* Sol Kart - Profil Özeti */}
            <div className="profile-main-card">
              <div className="profile-header-info">
                <div className="profile-avatar-giant">
                  {profile?.firstName?.[0]}{profile?.lastName?.[0]}
                </div>
                <h2>{profile?.firstName} {profile?.lastName}</h2>
                <span className="profile-role-tag">
                  <BadgeCheck size={14} /> {profile?.role === 'Admin' ? 'Yönetici' : 'Müşteri'}
                </span>
              </div>

              <div className="profile-stats-mini">
                <div className="stat-item">
                  <Clock size={18} />
                  <div>
                    <label>Kayıt Tarihi</label>
                    <span>{formatDate(profile?.createdAtUtc)}</span>
                  </div>
                </div>
              </div>
            </div>

            {/* Sağ Kart - Detaylı Bilgiler */}
            <div className="profile-details-card">
              <h3>Hesap Detayları</h3>
              <div className="details-list">
                <div className="detail-row">
                  <div className="detail-icon"><UserIcon size={20} /></div>
                  <div className="detail-content">
                    <label>Ad Soyad</label>
                    <p>{profile?.firstName} {profile?.lastName}</p>
                  </div>
                </div>

                <div className="detail-row">
                  <div className="detail-icon"><Mail size={20} /></div>
                  <div className="detail-content">
                    <label>E-posta Adresi</label>
                    <p>{profile?.email}</p>
                  </div>
                </div>

                <div className="detail-row">
                  <div className="detail-icon"><Calendar size={20} /></div>
                  <div className="detail-content">
                    <label>Sisteme Katılış</label>
                    <p>{formatDate(profile?.createdAtUtc)}</p>
                  </div>
                </div>

                <div className="detail-row">
                  <div className="detail-icon"><Shield size={20} /></div>
                  <div className="detail-content">
                    <label>Yetki Seviyesi</label>
                    <p>{profile?.role}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </section>
      </main>
    </div>
  );
};

export default Profile;
