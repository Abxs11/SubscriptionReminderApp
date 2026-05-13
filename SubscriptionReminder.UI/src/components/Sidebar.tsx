import {
  CreditCard,
  Bell,
  User as UserIcon,
  TrendingUp,
  LogOut,
  Shield,
  Trash2,
  Wallet
} from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

interface SidebarProps {
  activePage: 'dashboard' | 'subscriptions' | 'reminders' | 'profile' | 'admin' | 'payments';
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

  const handleDeleteAccount = async () => {
    if (window.confirm('Hesabınızı silmek istediğinizden emin misiniz? Tüm abonelikleriniz ve verileriniz kalıcı olarak silinecektir!')) {
      try {
        await api.delete('/Customers/me');
        alert('Hesabınız başarıyla silindi.');
        handleLogout();
      } catch (error) {
        console.error('Hesap silme hatası:', error);
        alert('Hesap silinirken bir hata oluştu.');
      }
    }
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
            <div
              className={`nav-item ${activePage === 'payments' ? 'active' : ''}`}
              onClick={() => navigate('/payments')}
            >
              <Wallet size={22} /> <span>Ödeme Geçmişi</span>
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
        {!isAdmin && (
          <button onClick={handleDeleteAccount} className="delete-account-btn">
            <Trash2 size={18} />
            <span>Hesabımı Sil</span>
          </button>
        )}
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
