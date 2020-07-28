import React, { useState } from 'react';
import DocumentUpload from './DocumentUpload';
import StatsTable from './StatsTable';
import DocumentDialog from './DocumentDialog';

export default function Home(props) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedId, setSelectedId] = useState(null);
  const [statsIsStale, setStatsIsStale] = useState(true);

  const onDocumentRowClick = (id) => {
    console.log(`click: ${id}`);
    setSelectedId(id);
    setDialogOpen(true);
  };

  const onDocumentDialogClose = () => {
    setDialogOpen(false);
    setSelectedId(null);
  };

  return (
    <div>
      <DocumentUpload onSuccess={() => setStatsIsStale(true)} />
      <StatsTable onDocumentRowClick={onDocumentRowClick} isStale={statsIsStale} onFetched={() => setStatsIsStale(false)} />
      <DocumentDialog id={selectedId} open={dialogOpen} onClose={onDocumentDialogClose} />
    </div>
  );
}
