import React from 'react';
import {
  CreditCard,
  Bell,
  User as UserIcon,
  TrendingUp,
  LogOut,
  Shield
} from 'lucide-react';
import { useNavigate } from 'react-router-dom';

interface SidebarProps {
  activePage: 'dashboard' | 'subscriptions' | 'reminders' | 'profile' | 'admin';
}

const Sidebar: React.FC<SidebarProps> = ({ activePage }) => {
  const navigate = useNavigate();
  const user = JSON.parse(localStorage.getItem('user') || '{}');
  const isAdmin = user.role === 'Admin';

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    navigate('/login');
  };

  return (
    <aside className="sidebar premium-sidebar">
      <div className="sidebar-logo">
        <div className="logo-box">SR</div>
        <span>RemindMe</span>
      </div>

      <nav className="sidebar-nav">
        {isAdmin ? (
          <div
            className={`nav-item ${activePage === 'admin' ? 'active' : ''}`}
            onClick={() => navigate('/admin')}
          >
            <Shield size={22} /> <span>Yönetim Paneli</span>
          </div>
        ) : (
          <div
            className={`nav-item ${activePage === 'dashboard' ? 'active' : ''}`}
            onClick={() => navigate('/dashboard')}
          >
            <TrendingUp size={22} /> <span>Genel Bakış</span>
          </div>
        )}

        {!isAdmin && (
          <>
            <div
              className={`nav-item ${activePage === 'subscriptions' ? 'active' : ''}`}
              onClick={() => navigate('/subscriptions')}
            >
              <CreditCard size={22} /> <span>Aboneliklerim</span>
            </div>
            <div
              className={`nav-item ${activePage === 'reminders' ? 'active' : ''}`}
              onClick={() => navigate('/reminders')}
            >
              <Bell size={22} /> <span>Hatırlatıcılar</span>
            </div>
          </>
        )}

        <div
          className={`nav-item ${activePage === 'profile' ? 'active' : ''}`}
          onClick={() => navigate('/profile')}
        >
          <UserIcon size={22} /> <span>Profilim</span>
        </div>
      </nav>

      <div className="sidebar-footer">
        <button onClick={handleLogout} className="premium-logout-btn">
          <div className="btn-content">
            <LogOut size={20} />
            <span>Çıkış Yap</span>
          </div>
          <div className="btn-glow"></div>
        </button>
      </div>
    </aside>
  );
};

export default Sidebar;
