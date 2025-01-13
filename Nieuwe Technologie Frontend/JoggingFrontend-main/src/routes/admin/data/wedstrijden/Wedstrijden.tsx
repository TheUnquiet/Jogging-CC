import { useState, useCallback, useEffect } from 'react';
import MyCardHeader from '@/components/cards/cardHeaders/CardHeaderForTables';
import { Card, CardContent } from '@/components/ui/card';
import CompetitionsTable from '@/components/tables/competitionTable/CompetitionsTable';
import CompetitionForm from '@/components/forms/competitionForm/CompetitionForm';
import { Competition } from '@/types';

export default function Wedstrijden() {
  const [open, setOpen] = useState(false);
  const [competition, setCompetition] = useState<Competition | null>(null);
  const [competitions, setCompetitions] = useState<Competition[]>([]);

  const handleEditOpen = useCallback((contest: Competition) => {
    setCompetition(contest);
    setOpen(true);
  }, []);

  const toggleOpen = useCallback(() => setOpen((o) => !o), []);

  const fetchActiveContests = useCallback(async () => {
    try {
      const url = '/api/competition'; // Definieer de URL als een string
      console.log('Fetching from URL:', url); // Log de URL
      const response = await fetch(url);
      let data = await response.json();

      // Controleer wat de API retourneert
      console.log('API response:', data);

      // Controleer of data een array is
      if (!Array.isArray(data)) {
        data = [];
      }

      const activeContests = data.filter((contest: Competition) => contest.active);
      setCompetitions(activeContests);
    } catch (error) {
      console.error('Error fetching active contests:', error);
    }
  }, []);

  useEffect(() => {
    fetchActiveContests();
  }, [fetchActiveContests]);

  return (
    <Card>
      {open ? (
        <>
          <MyCardHeader
            title={competition?.id ? 'Bewerk' : 'Voeg toe'}
            onClick={toggleOpen}
            toggleSign='negative'
          />
          <CardContent>
            <CompetitionForm
              competitionId={competition?.id}
              onClick={toggleOpen}
            />
          </CardContent>
        </>
      ) : (
        <CompetitionsTable competitions={competitions} onEdit={handleEditOpen} />
      )}
    </Card>
  );
}