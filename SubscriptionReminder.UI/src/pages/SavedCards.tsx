import React, { useEffect, useState } from 'react';
import api from '../services/api';
import './Pages.css';
import './SavedCards.css';
import Sidebar from '../components/Sidebar';
import { CreditCard, Trash2, Plus, X } from 'lucide-react';

const SavedCards: React.FC = () => {
  const [cards, setCards] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [isAdding, setIsAdding] = useState(false);
  const [formData, setFormData] = useState({
    cardHolderName: '',
    fullCardNumber: '',
    expiryDate: '',
    cvv: ''
  });
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    fetchCards();
  }, []);

  const fetchCards = async () => {
    try {
      const response = await api.get('/SavedCards');
      setCards(response.data);
    } catch (err) {
      console.error('Kartlar yüklenemedi:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Bu kartı silmek istediğinize emin misiniz?')) return;
    try {
      await api.delete(`/SavedCards/${id}`);
      setCards(cards.filter(c => c.id !== id));
    } catch (err) {
      alert('Silme işlemi başarısız oldu.');
    }
  };

  const formatCardNumber = (value: string) => {
    const v = value.replace(/\s+/g, '').replace(/[^0-9]/gi, '');
    const matches = v.match(/\d{4,16}/g);
    const match = matches && matches[0] || '';
    const parts = [];
    for (let i = 0, len = match.length; i < len; i += 4) {
      parts.push(match.substring(i, i + 4));
    }
    if (parts.length) {
      return parts.join(' ');
    } else {
      return value;
    }
  };

  const formatExpiry = (value: string) => {
    const v = value.replace(/\s+/g, '').replace(/[^0-9]/gi, '');
    if (v.length >= 2) {
      return `${v.substring(0, 2)}/${v.substring(2, 4)}`;
    }
    return v;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;

    if (name === 'fullCardNumber') {
      setFormData({ ...formData, [name]: formatCardNumber(value) });
    } else if (name === 'expiryDate') {
      setFormData({ ...formData, [name]: formatExpiry(value) });
    } else {
      setFormData({ ...formData, [name]: value });
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    const unformattedCardNumber = formData.fullCardNumber.replace(/\s/g, '');
    if (unformattedCardNumber.length !== 16) {
      setError('Kart numarası 16 haneli olmalıdır.');
      return;
    }

    // Son Kullanma Tarihi Validasyonu
    if (formData.expiryDate.length === 5) {
      const [monthStr, yearStr] = formData.expiryDate.split('/');
      const expMonth = parseInt(monthStr, 10);
      const expYear = parseInt(yearStr, 10) + 2000; // e.g. "28" -> 2028
      
      const currentDate = new Date();
      const currentYear = currentDate.getFullYear();
      const currentMonth = currentDate.getMonth() + 1; // 0-indexed

      if (expYear < currentYear || (expYear === currentYear && expMonth < currentMonth)) {
        setError('Son kullanma tarihi geçmiş bir kart ekleyemezsiniz.');
        return;
      }

      if (expMonth < 1 || expMonth > 12) {
        setError('Geçersiz bir ay girdiniz (01-12 olmalı).');
        return;
      }
    } else {
      setError('Lütfen son kullanma tarihini AA/YY formatında tam olarak girin.');
      return;
    }

    setSubmitting(true);
    try {
      const requestData = {
        ...formData,
        fullCardNumber: unformattedCardNumber
      };

      const response = await api.post('/SavedCards', requestData);
      setCards([response.data, ...cards]);
      setIsAdding(false);
      setFormData({ cardHolderName: '', fullCardNumber: '', expiryDate: '', cvv: '' });
    } catch (err: any) {
      setError(err.response?.data?.message || 'Kart eklenirken bir hata oluştu.');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="dashboard-layout">
      <Sidebar activePage="saved-cards" />
      <main className="main-content">
        <header className="content-header">
          <div className="header-title">
            <h1>Kayıtlı Kartlarım</h1>
          </div>
        </header>

        <section className="page-content animate-fade-in">
          <div className="section-header">
            <div>
              <h2>Ödeme Yöntemleri</h2>
              <p>Kart bilgileriniz PCI-DSS standartlarına uygun olarak maskelenerek saklanmaktadır.</p>
            </div>
            {!isAdding && (
              <button className="add-btn" onClick={() => setIsAdding(true)}>
                <Plus size={18} /> Yeni Kart Ekle
              </button>
            )}
          </div>

          {isAdding && (
            <div className="add-card-container animate-fade-in">
              <div className="add-card-header">
                <h3>Yeni Kredi/Banka Kartı</h3>
                <button className="close-btn" onClick={() => setIsAdding(false)}><X size={20} /></button>
              </div>
              <form onSubmit={handleSubmit} className="add-card-form">
                {error && <div className="error-message">{error}</div>}
                <div className="form-group">
                  <label>Kart Üzerindeki İsim</label>
                  <input
                    type="text"
                    name="cardHolderName"
                    value={formData.cardHolderName}
                    onChange={handleChange}
                    placeholder="JOHN DOE"
                    required
                    maxLength={150}
                  />
                </div>
                <div className="form-group">
                  <label>Kart Numarası</label>
                  <input
                    type="text"
                    name="fullCardNumber"
                    value={formData.fullCardNumber}
                    onChange={handleChange}
                    placeholder="0000 0000 0000 0000"
                    required
                    maxLength={19}
                  />
                </div>
                <div className="form-row">
                  <div className="form-group">
                    <label>Son Kullanma (AA/YY)</label>
                    <input
                      type="text"
                      name="expiryDate"
                      value={formData.expiryDate}
                      onChange={handleChange}
                      placeholder="MM/YY"
                      required
                      maxLength={5}
                    />
                  </div>
                  <div className="form-group">
                    <label>CVV</label>
                    <input
                      type="password"
                      name="cvv"
                      value={formData.cvv}
                      onChange={handleChange}
                      placeholder="***"
                      required
                      maxLength={3}
                    />
                    <small className="hint">Güvenliğiniz için CVV kaydedilmez.</small>
                  </div>
                </div>
                <div className="form-actions">
                  <button type="button" className="btn-secondary" onClick={() => setIsAdding(false)}>İptal</button>
                  <button type="submit" className="btn-primary" disabled={submitting}>
                    {submitting ? 'Ekleniyor...' : 'Kartı Kaydet'}
                  </button>
                </div>
              </form>
            </div>
          )}

          <div className="cards-grid">
            {cards.map(card => (
              <div className="credit-card-item" key={card.id}>
                <div className="cc-bg-pattern"></div>
                <div className="cc-header">
                  <CreditCard size={28} className="cc-chip-icon" />
                  <span className="cc-brand">VISA</span>
                </div>
                <div className="cc-number">
                  {card.maskedCardNumber}
                </div>
                <div className="cc-footer">
                  <div className="cc-holder">
                    <span>Card Holder</span>
                    <p>{card.cardHolderName}</p>
                  </div>
                  <div className="cc-expires">
                    <span>Expires</span>
                    <p>{card.expiryDate}</p>
                  </div>
                </div>
                <button className="cc-delete-btn" onClick={() => handleDelete(card.id)} title="Kartı Sil">
                  <Trash2 size={16} />
                </button>
              </div>
            ))}

            {cards.length === 0 && !loading && !isAdding && (
              <div className="empty-state-container">
                <p>Henüz kayıtlı bir kartınız bulunmuyor.</p>
              </div>
            )}
          </div>
        </section>
      </main>
    </div>
  );
};

export default SavedCards;
