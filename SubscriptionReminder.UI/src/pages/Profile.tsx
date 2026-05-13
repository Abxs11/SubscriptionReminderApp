import React from 'react';
import Sidebar from '../components/Sidebar';
import './Pages.css';
import { User as UserIcon, Mail, Shield, Calendar, Phone } from 'lucide-react';

const Profile: React.FC = () => {
  const user = JSON.parse(localStorage.getItem('user') || '{}');

  return (
    <div className="dashboard-layout">
      <Sidebar activePage="profile" />
      <main className="main-content">
        <header className="content-header-premium">
          <div className="page-title">
            <UserIcon size={24} color="#8b5cf6" />
            <h1>Profil Bilgilerim</h1>
          </div>
        </header>

        <section className="page-content animate-fade-in">
          <div className="profile-container-premium">
            <div className="profile-card-large">
              <div className="profile-avatar-giant">
                {user.email?.[0].toUpperCase()}
              </div>
              <div className="profile-details-list">
                <div className="profile-detail-item">
                  <Mail size={20} />
                  <div className="detail-info">
                    <label>E-posta Adresi</label>
                    <span>{user.email}</span>
                  </div>
                </div>
                <div className="profile-detail-item">
                  <Shield size={20} />
                  <div className="detail-info">
                    <label>Kullanıcı Rolü</label>
                    <span className="role-badge">{user.role}</span>
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
