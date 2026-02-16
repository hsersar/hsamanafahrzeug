import type { StatusHistorieEintrag } from '../../types';
import './TrackingTimeline.css';

interface Props {
  historie: StatusHistorieEintrag[];
}

export default function TrackingTimeline({ historie }: Props) {
  const sortedHistorie = [...historie].sort(
    (a, b) => new Date(a.zeitpunkt).getTime() - new Date(b.zeitpunkt).getTime()
  );

  return (
    <div className="tracking-timeline">
      <h3>📋 Statusverlauf</h3>
      <div className="timeline">
        {sortedHistorie.map((eintrag, index) => (
          <div
            key={index}
            className={`timeline-item ${eintrag.istAktuell ? 'current' : 'completed'}`}
          >
            <div className="timeline-marker">
              {eintrag.istAktuell ? '🔄' : '✅'}
            </div>
            <div className="timeline-content">
              <h4>{eintrag.beschreibung}</h4>
              <p className="timeline-date">
                {new Date(eintrag.zeitpunkt).toLocaleString('de-DE', {
                  dateStyle: 'medium',
                  timeStyle: 'short',
                })}
              </p>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
