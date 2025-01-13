import React, { useEffect, useState, useCallback } from 'react';
import {
    Table,
    TableHeader,
    TableBody,
    TableRow,
    TableCell,
} from '@/components/ui/table';
import ContestRow from './ContestRow';
import { Competition } from '@/types';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';

interface CompetitionsTableProps {
    competitions: Competition[];
    onEdit: (contest: Competition) => void;
}

const CompetitionsTable: React.FC<CompetitionsTableProps> = ({ competitions, onEdit }) => {
    const [page, setPage] = useState<number>(1);
    const [pageSize] = useState<number>(6);
    const [totalPages, setTotalPages] = useState<number>(0);
    const [searchValue, setSearchValue] = useState<string>('');
    const [startDate, setStartDate] = useState<string>('');
    const [endDate, setEndDate] = useState<string>('');

    const filteredCompetitions = competitions.filter((contest) => {
        const matchesSearch = contest.name.toLowerCase().includes(searchValue.toLowerCase());
        const matchesStartDate = startDate ? new Date(contest.date) >= new Date(startDate) : true;
        const matchesEndDate = endDate ? new Date(contest.date) <= new Date(endDate) : true;
        return matchesSearch && matchesStartDate && matchesEndDate;
    });

    const paginatedCompetitions = filteredCompetitions.slice((page - 1) * pageSize, page * pageSize);

    useEffect(() => {
        setTotalPages(Math.ceil(filteredCompetitions.length / pageSize));
    }, [filteredCompetitions, pageSize]);

    const handlePrevPage = () => {
        if (page > 1) {
            setPage(page - 1);
        }
    };

    const handleNextPage = () => {
        if (page < totalPages) {
            setPage(page + 1);
        }
    };

    const debounce = (func: (...args: any[]) => void, wait: number) => {
        let timeout: ReturnType<typeof setTimeout>;
        return (...args: any[]) => {
            clearTimeout(timeout);
            timeout = setTimeout(() => func(...args), wait);
        };
    };

    const debouncedSearch = useCallback(
        debounce((value: string) => {
            setSearchValue(value);
            setPage(1); // Reset to first page on search
        }, 500),
        []
    );

    const handleStartDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setStartDate(e.target.value);
        setPage(1); // Reset to first page on date change
    };

    const handleEndDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setEndDate(e.target.value);
        setPage(1); // Reset to first page on date change
    };

    return (
        <>
            <div className='flex flex-col items-center justify-between gap-3 pb-3 lg:flex-row'>
                <Input
                    placeholder='Zoek wedstrijden...'
                    onChange={(e) => debouncedSearch(e.target.value)}
                />
                <div className='flex items-center justify-between w-full gap-2'>
                    <label className='w-[50px] font-semibold'>Start: </label>
                    <Input
                        className='w-full'
                        type='date'
                        max='9999-12-31'
                        placeholder='Start Date '
                        onChange={handleStartDateChange}
                    />
                </div>
                <div className='flex items-center justify-around w-full gap-2'>
                    <label className='w-[50px] font-semibold'>Eind: </label>
                    <Input
                        className='w-full'
                        type='date'
                        max='9999-12-31'
                        placeholder='End Date'
                        onChange={handleEndDateChange}
                    />
                </div>
            </div>
            <Table>
                <TableHeader>
                    <TableRow>
                        <TableCell>Naam</TableCell>
                        <TableCell>Datum</TableCell>
                        <TableCell>Status</TableCell>
                        <TableCell>Ranking</TableCell>
                        <TableCell>Actions</TableCell>
                    </TableRow>
                </TableHeader>
                <TableBody className='w-full'>
                    {paginatedCompetitions.map((contest) => (
                        <ContestRow
                            key={contest.id}
                            contest={contest}
                            refreshContests={() => {}}
                            onEdit={onEdit}
                        />
                    ))}
                </TableBody>
            </Table>
            <div className='flex items-center justify-between gap-2 mt-4'>
                <Button onClick={handlePrevPage} disabled={page === 1}>
                    Vorige
                </Button>
                <p>
                    {page} / {totalPages}
                </p>
                <Button onClick={handleNextPage} disabled={page >= totalPages}>
                    Volgende
                </Button>
            </div>
        </>
    );
};

export default CompetitionsTable;