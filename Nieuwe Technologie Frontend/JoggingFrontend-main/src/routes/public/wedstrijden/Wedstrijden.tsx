import React, { useEffect, useState } from "react";
import ActiveContestCard from "@/components/cards/ActiveContestCard";
import { fetchPublicCompetitions } from "@/services/CompetitionService";
import { Competition } from "@/types";
import { Link } from "react-router-dom";
import { Button } from "@/components/ui/button";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
  type CarouselApi,
} from "@/components/ui/carousel";
import useWindowWidth from "@/hooks/useWindowWidth";

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
    <div className="flex justify-center mt-4">
      {Array.from({ length: total }).map((_, index) => (
        <button
          key={index}
          onClick={() => onDotClick(index)}
          className={`w-3 h-3 mx-1 rounded-full ${
            index === currentIndex ? "bg-blue-500" : "bg-gray-300"
          }`}
          style={{
            width: index === currentIndex ? "8px" : "6px",
            height: index === currentIndex ? "8px" : "6px",
          }}
        />
      ))}
    </div>
  );
}

export default function Wedstrijden() {
  const [todayCompetitions, setTodayCompetitions] = useState<Competition[]>([]);
  const [upcomingCompetitions, setUpcomingCompetitions] = useState<
    Competition[]
  >([]);
  const [pastCompetitions, setPastCompetitions] = useState<Competition[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const windowWidth = useWindowWidth();
  const [api, setApi] = useState<CarouselApi>();
  const [currentIndex, setCurrentIndex] = useState(0);
  const [count, setCount] = useState(0);
  const [autoplay, setAutoplay] = useState(true);

  // Define the style only if windowWidth is less than 768px
  const maxWidthStyle =
    windowWidth < 768 ? { maxWidth: `${windowWidth}px` } : {};

  // Helper function to check if two dates are the same day
  const isSameDay = (date1: Date, date2: Date) => {
    return (
      date1.getDate() === date2.getDate() &&
      date1.getMonth() === date2.getMonth() &&
      date1.getFullYear() === date2.getFullYear()
    );
  };

  useEffect(() => {
    const fetchActiveContests = async () => {
      try {
        const { data } = await fetchPublicCompetitions();
        const now = new Date();

        const todayComps = data.filter((comp: Competition) =>
          isSameDay(new Date(comp.date), now)
        );
        const upcoming = data.filter(
          (comp: Competition) => new Date(comp.date) > now
        );
        const past = data.filter(
          (comp: Competition) =>
            new Date(comp.date) < now && !isSameDay(new Date(comp.date), now)
        );

        upcoming.sort(
          (a: Competition, b: Competition) =>
            new Date(a.date).getTime() - new Date(b.date).getTime()
        );
        past.sort(
          (a: Competition, b: Competition) =>
            new Date(b.date).getTime() - new Date(a.date).getTime()
        );

        setTodayCompetitions(todayComps);
        setUpcomingCompetitions(upcoming);
        setPastCompetitions(past);
      } catch (error: any) {
        console.error("Error fetching active contests:", error);
        setError("Failed to fetch contests");
      } finally {
        setLoading(false);
      }
    };

    fetchActiveContests();
  }, []);

  useEffect(() => {
    if (!api) {
      return;
    }

    setCount(api.scrollSnapList().length);
    setCurrentIndex(api.selectedScrollSnap());

    api.on("select", () => {
      setCurrentIndex(api.selectedScrollSnap());
    });

    const stopAutoplay = () => setAutoplay(false);

    api.on("pointerDown", stopAutoplay);

    return () => {
      api.off("pointerDown", stopAutoplay);
    };
  }, [api]);

  useEffect(() => {
    if (!autoplay) return;

    const interval = setInterval(() => {
      if (api) {
        if (currentIndex === count - 1) {
          api.scrollTo(0);
        } else {
          api.scrollNext();
        }
      }
    }, 3000);

    return () => clearInterval(interval); // Cleanup interval on component unmount or autoplay stop
  }, [api, autoplay, currentIndex, count]);

  const handleDotClick = (index: number) => {
    if (api) {
      api.scrollTo(index);
    }
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <div className="flex flex-col items-center space-y-6">
      <h1 className="text-5xl font-semibold text-center" id="#joggings">
        Wedstrijden
      </h1>
      <h2 className="text-2xl text-center">Het Augustijn Criterium</h2>
      <p className="text-center">
        Deze joggings maken deel uit van het Augustijn Criterium, met een totale
        prijzenpot van €1000 en vele naturaprijzen.
      </p>
      <div className="flex flex-wrap justify-center gap-4">
        {todayCompetitions.length > 0 && (
          <div className="flex flex-col items-center justify-center gap-3 p-6 text-center border shadow-lg w-fit md:flex-row rounded-xl bg-slate-200 dark:bg-slate-700">
            <div className="flex items-center justify-center w-full lg:w-1/2">
              {todayCompetitions.map((competition) => (
                <Link key={competition.id} to={`/wedstrijd/${competition.id}`}>
                  <ActiveContestCard competition={competition} />
                </Link>
              ))}
            </div>
            <div className="flex flex-col items-center gap-3 h-3/4 justify-evenly w-fit">
              <h3 className="text-2xl font-bold text-center md:w-3/4 md:text-4xl lg:text-5xl drop-shadow-2xl">
                Schrijf je in voor de competitie van vandaag!
              </h3>
              {todayCompetitions.map((competition) => (
                <Link key={competition.id} to={`/wedstrijd/${competition.id}`}>
                  <Button className="w-full md:w-fit ">Schrijf je in</Button>
                </Link>
              ))}
            </div>
          </div>
        )}

        {/* Show Carousel for small screens */}
        <div className="flex flex-col md:hidden">
          <Carousel setApi={setApi}>
            <CarouselContent
              style={maxWidthStyle}
              className={`max-w-[${maxWidthStyle}px] md:max-w-3xl`}
            >
              {upcomingCompetitions.map((competition) => (
                <CarouselItem
                  key={competition.id}
                  className="flex justify-center"
                >
                  <Link to={`/wedstrijd/${competition.id}`}>
                    <ActiveContestCard competition={competition} />
                  </Link>
                </CarouselItem>
              ))}
              {pastCompetitions.map((competition) => (
                <CarouselItem
                  key={competition.id}
                  className="flex justify-center"
                >
                  <Link to={`/wedstrijd/${competition.id}`}>
                    <ActiveContestCard competition={competition} />
                  </Link>
                </CarouselItem>
              ))}
            </CarouselContent>
            {/* <CarouselPrevious />
            <CarouselNext /> */}
          </Carousel>
          <PaginationDots
            total={count}
            currentIndex={currentIndex}
            onDotClick={handleDotClick}
          />
        </div>

        {/* Show list for medium and larger screens */}
        <div className="flex-wrap justify-center hidden gap-4 md:flex">
          {upcomingCompetitions.map((competition) => (
            <Link key={competition.id} to={`/wedstrijd/${competition.id}`}>
              <ActiveContestCard competition={competition} />
            </Link>
          ))}
          {pastCompetitions.map((competition) => (
            <Link key={competition.id} to={`/wedstrijd/${competition.id}`}>
              <ActiveContestCard competition={competition} />
            </Link>
          ))}
        </div>
      </div>
    </div>
  );
}
