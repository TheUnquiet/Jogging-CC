import React from "react";
import SimpleNavBar from "@/components/nav/SimpleNavBar";

export default function Uitslagen() {
  return (
    <div className="flex flex-col min-h-screen bg-background text-foreground">
      <SimpleNavBar />
      <div className="flex flex-col items-center space-y-6 p-6 flex-grow">
        <h1 className="text-5xl font-semibold text-center " id="joggings">
          Uitslagen
        </h1>
        <h2 className="text-2xl text-center text-secondary-foreground">
          Subtitel
        </h2>
        <p className="text-center text-muted-foreground max-w-2xl">
          Hier vindt u de uitslagen van de verschillende wedstrijden.
        </p>
        <div className="w-full h-64 bg-card flex items-center justify-center rounded shadow">
          <p className="text-card-foreground">Inhoud van de uitslagen</p>
        </div>
      </div>
    </div>
  );
}
