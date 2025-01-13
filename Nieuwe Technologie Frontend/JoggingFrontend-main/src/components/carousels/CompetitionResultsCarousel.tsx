import * as React from 'react';
import {
    Carousel,
    CarouselContent,
    CarouselItem,
    CarouselNext,
    CarouselPrevious,
    type CarouselApi,
} from '@/components/ui/carousel';
import ClassificationsTable from '../tables/classificationTable/ClassificationTable';
import useWindowWidth from '@/hooks/useWindowWidth';
import { Button } from '../ui/button';
import { Link } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { RankingCategory } from '@/types';
import { fetchRankings } from '@/services/RankingService';

type PaginationDotsProps = {
    total: number;
    currentIndex: number;
    onDotClick: (index: number) => void;
};

function PaginationDots({
    total,
    currentIndex,
    onDotClick,
}: PaginationDotsProps) {
    return (
        <div className='flex justify-center mt-4'>
            {Array.from({ length: total }).map((_, index) => (
                <button
                    key={index}
                    onClick={() => onDotClick(index)}
                    className={`w-3 h-3 mx-1 rounded-full ${
                        index === currentIndex ? 'bg-blue-500' : 'bg-gray-300'
                    }`}
                    style={{
                        width: index === currentIndex ? '8px' : '6px',
                        height: index === currentIndex ? '8px' : '6px',
                    }}
                />
            ))}
        </div>
    );
}

export function CompetitionResultsCarousel() {
    const [api, setApi] = React.useState<CarouselApi>();
    const [currentIndex, setCurrentIndex] = React.useState(0);
    const [count, setCount] = React.useState(0);
    const [autoplay, setAutoplay] = React.useState(true);
    const [rankings, setRankings] = useState<{ [key: string]: RankingCategory[] }[]>([]);

    useEffect(() => {
        (async () => {
            try {
                const { data } = await fetchRankings({
                    pageSize: 2
                });

                if (!Array.isArray(data)) {
                    throw new Error('Expected an array');
                }

                const groupedData = groupedEntries(data);
                setCount(groupedData.length + 1);
                setRankings(groupedData);
            } catch (error) {
                console.error('Failed to fetch rankings', error);
            }
        })();
    }, []);

    React.useEffect(() => {
        if (!api) {
            return;
        }

        setCount(api.scrollSnapList().length);
        setCurrentIndex(api.selectedScrollSnap());

        api.on('select', () => {
            setCurrentIndex(api.selectedScrollSnap());
        });

        const stopAutoplay = () => setAutoplay(false);

        api.on('pointerDown', stopAutoplay);

        return () => {
            api.off('pointerDown', stopAutoplay);
        };
    }, [api]);

    React.useEffect(() => {
        if (!autoplay) return;

        const interval = setInterval(() => {
            if (api) {
                if (currentIndex === count - 1) {
                    api.scrollTo(0);
                } else {
                    api.scrollNext();
                }
            }
        }, 4000);

        return () => clearInterval(interval); // Cleanup interval on component unmount or autoplay stop
    }, [api, autoplay, currentIndex, count]);

    const handleDotClick = (index: number) => {
        if (api) {
            api.scrollTo(index);
        }
    };

    const groupedEntries = (data: RankingCategory[]) => {
        if (!Array.isArray(data)) {
            throw new Error('Expected an array');
        }

        return data.reduce((acc, entry) => {
            Object.keys(entry).forEach(key => {
                const [firstPart, secondPart, lastPart] = key.split('$');
                const groupKey = `${firstPart}$${lastPart}`;
                let object = acc.find(entry => entry[groupKey]);
                if (!object) {
                    acc.push({ [groupKey]: [] });
                    object = acc.find(entry => entry[groupKey]);
                }
                if (object) {
                    object[groupKey].push({ [secondPart]: entry[key] });
                }
            });
            return acc;
        }, [] as { [key: string]: RankingCategory[] }[]);
    };

    return (
        <div>
            <Carousel
                setApi={setApi}
                className='w-full max-w-sm sm:max-w-sm md:max-w-lg lg:max-w-xl'
            >
                <CarouselContent>
                    {rankings.map((ranking, index) => (
                        <CarouselItem key={index}>
                            <div className='p-3 '>
                                <ClassificationsTable ranking={ranking} />
                            </div>
                        </CarouselItem>
                    ))}
                    <CarouselItem className='flex items-center justify-center'>
                        <Link
                            to={`/klassementen`}
                            className='flex items-center justify-center gap-3 p-3 transition-transform duration-300 border rounded-lg shadow-md md:hover:scale-105 dark:bg-slate-950'
                        >
                            <p className='text-lg font-semibold'>Zie alle klassementen!</p>
                            <Button className='w-10 p-2'>
                                <svg
                                    width='15'
                                    height='15'
                                    viewBox='0 0 15 15'
                                    fill='none'
                                    xmlns='http://www.w3.org/2000/svg'
                                >
                                    <path
                                        d='M8.14645 3.14645C8.34171 2.95118 8.65829 2.95118 8.85355 3.14645L12.8536 7.14645C13.0488 7.34171 13.0488 7.65829 12.8536 7.85355L8.85355 11.8536C8.65829 12.0488 8.34171 12.0488 8.14645 11.8536C7.95118 11.6583 7.95118 11.3417 8.14645 11.1464L11.2929 8H2.5C2.22386 8 2 7.77614 2 7.5C2 7.22386 2.22386 7 2.5 7H11.2929L8.14645 3.85355C7.95118 3.65829 7.95118 3.34171 8.14645 3.14645Z'
                                        fill='currentColor'
                                        fillRule='evenodd'
                                        clipRule='evenodd'
                                    ></path>
                                </svg>
                            </Button>
                        </Link>
                    </CarouselItem>
                </CarouselContent>
                <div className='hidden md:block lg:block'>
                    <CarouselPrevious />
                    <CarouselNext />
                </div>
            </Carousel>
            <PaginationDots
                total={count}
                currentIndex={currentIndex}
                onDotClick={handleDotClick}
            />
        </div>
    );
}

export default CompetitionResultsCarousel;