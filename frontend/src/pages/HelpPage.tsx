import React, { useEffect, useState } from 'react';
import ApiService from '../services/api';
import { HelpArticle } from '../types';

const HelpPage: React.FC = () => {
  const [articles, setArticles] = useState<HelpArticle[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadHelp = async () => {
      try {
        setLoading(true);
        const data = await ApiService.getHelpArticles();
        setArticles(data);
        setError(null);
      } catch (err) {
        setError('Hilfe konnte nicht geladen werden.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    loadHelp();
  }, []);

  if (loading) {
    return <div className="loading">Hilfe wird geladen...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="page help-page">
      <div className="page-header">
        <div>
          <h1>Hilfe und Kontakt</h1>
          <p>Antworten auf häufige Fragen und Kontaktmöglichkeiten.</p>
        </div>
      </div>

      <div className="page-grid">
        {articles.map((article) => (
          <div key={article.id} className="card">
            <div className="card-header">
              <div>
                <div className="card-title">{article.title}</div>
                <div className="card-subtitle">{article.category}</div>
              </div>
              <span className="badge">
                {new Date(article.updatedAt).toLocaleDateString()}
              </span>
            </div>
            <p>{article.summary}</p>
            <p className="help-content">{article.content}</p>
          </div>
        ))}
      </div>
    </div>
  );
};

export default HelpPage;
