import { CompetitionResultsCarousel } from "@/components/carousels/CompetitionResultsCarousel";

export default function Klassementen() {
  return (
    <div className="flex flex-col items-center space-y-6">
      <h1 className="text-5xl font-semibold text-center" id="#joggings">
        Klassementen
      </h1>
      <p className="text-center">
        Hier vindt u de tijdelijke standen voor de verschillende Klassementen
      </p>
      <div className="flex flex-col items-center justify-center gap-3 justify-items-center">
        <CompetitionResultsCarousel />
      </div>
    </div>
  );
}
