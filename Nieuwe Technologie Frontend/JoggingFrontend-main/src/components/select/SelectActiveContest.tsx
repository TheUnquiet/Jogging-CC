import {
    Select,
    SelectContent,
    SelectGroup,
    SelectItem,
    SelectLabel,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import { useEffect, useState } from 'react';
import { fetchPublicCompetitions } from '@/services/CompetitionService';
import { Competition } from '@/types';

interface SelectActiveContestProps {
    onCompetitionSelect: (competitionId: number) => void;
}

export function SelectActiveContest({
    onCompetitionSelect,
}: SelectActiveContestProps) {
    const [activeCompetitions, setActiveCompetitions] = useState<Competition[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [selectedCompetitionName, setSelectedCompetitionName] = useState<string | null>(null);

    useEffect(() => {
        const fetchActiveContests = async () => {
            try {
                const { data: competitions } = await fetchPublicCompetitions();

                if (!Array.isArray(competitions)) {
                    throw new Error('Expected an array');
                }

                setActiveCompetitions(competitions);
                const savedCompetitionName = localStorage.getItem('selectedCompetitionName');
                if (savedCompetitionName) {
                    const selectedCompetition = competitions.find(
                        (competition: Competition) => competition.name === savedCompetitionName
                    );
                    if (selectedCompetition) {
                        setSelectedCompetitionName(savedCompetitionName);
                        onCompetitionSelect(selectedCompetition.id);
                    }
                }
            } catch (error: any) {
                console.error('Error fetching active contests:', error);
                setError('Failed to fetch contests');
            }
        };

        fetchActiveContests();
    }, []);

    const handleSelect = (value: string) => {
        const selectedCompetition = activeCompetitions.find(
            (competition) => competition.name === value
        );
        if (selectedCompetition) {
            setSelectedCompetitionName(selectedCompetition.name);
            localStorage.setItem('selectedCompetitionName', selectedCompetition.name);
            onCompetitionSelect(selectedCompetition.id);
        }
    };

    return (
        <Select onValueChange={handleSelect} value={selectedCompetitionName || ''}>
            <SelectTrigger>
                <SelectValue placeholder='Selecteer wedstrijd' />
            </SelectTrigger>
            <SelectContent>
                <SelectGroup>
                    <SelectLabel>Actieve Wedstrijden</SelectLabel>
                    {activeCompetitions.map((competition) => (
                        <SelectItem key={competition.id} value={competition.name}>
                            {competition.name}
                        </SelectItem>
                    ))}
                </SelectGroup>
            </SelectContent>
        </Select>
    );
}