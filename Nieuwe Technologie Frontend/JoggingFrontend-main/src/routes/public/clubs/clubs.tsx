import React, { useEffect, useState } from "react";
import SimpleNavBar from "@/components/nav/SimpleNavBar";

interface Club {
  name: string;
  image: string;
  members: Member[];
}

interface Member {
  position: number;
  name: string;
  short: string;
  middle: string;
  long: string;
  category: string;
  gender: string;
}

const clubsData: Club[] = [
  {
    name: "Kozirunners",
    image: "https://via.placeholder.com/150", // Voeg hier de URL van de afbeelding toe
    members: [
      {
        position: 1,
        name: "Stijn Van De Voorde",
        short: "18:37",
        middle: "39:15",
        long: "01:10:12",
        category: "+35",
        gender: "M",
      },
      // Voeg hier meer leden toe
    ],
  },
  {
    name: "Lucky runners",
    image: "https://via.placeholder.com/150", // Voeg hier de URL van de afbeelding toe
    members: [
      {
        position: 1,
        name: "Ann-Sophie Henderick",
        short: "18:37",
        middle: "39:15",
        long: "01:10:12",
        category: "+35",
        gender: "V",
      },
      // Voeg hier meer leden toe
    ],
  },
  // Voeg hier meer clubs toe
];

export const Clubs: React.FC = () => {
  const [clubs, setClubs] = useState<Club[]>([]);

  useEffect(() => {
    // Simuleer een API-aanroep
    setClubs(clubsData);
  }, []);

  return (
    <div className="flex flex-col min-h-screen bg-background text-foreground">
      <SimpleNavBar />
      <div className="flex flex-col items-center space-y-6 p-6 flex-grow">
        <h1 className="text-5xl font-semibold text-center " id="joggings">
          Clubs
        </h1>
        {clubs.map((club) => (
          <div
            key={club.name}
            className="w-full max-w-4xl bg-card text-card-foreground rounded shadow p-4 mb-6"
          >
            <div className="flex items-center space-x-4">
              <img
                src={club.image}
                alt={club.name}
                className="w-24 h-24 rounded-full"
              />
              <h2 className="text-3xl font-semibold text-center mb-4 text-card-foreground">
                {club.name}
              </h2>
            </div>
            <table className="min-w-full bg-card">
              <thead>
                <tr className="bg-secondary text-secondary-foreground">
                  <th className="py-2 px-4 border-b border-border">Positie</th>
                  <th className="py-2 px-4 border-b border-border">Naam</th>
                  <th className="py-2 px-4 border-b border-border">Kort</th>
                  <th className="py-2 px-4 border-b border-border">Midden</th>
                  <th className="py-2 px-4 border-b border-border">Lang</th>
                  <th className="py-2 px-4 border-b border-border">Cat</th>
                  <th className="py-2 px-4 border-b border-border">Geslacht</th>
                </tr>
              </thead>
              <tbody>
                {club.members.map((member) => (
                  <tr
                    key={member.name}
                    className="hover:bg-muted hover:text-muted-foreground"
                  >
                    <td className="py-2 px-4 border-b border-border text-center">
                      {member.position}
                    </td>
                    <td className="py-2 px-4 border-b border-border">
                      {member.name}
                    </td>
                    <td className="py-2 px-4 border-b border-border text-center">
                      {member.short}
                    </td>
                    <td className="py-2 px-4 border-b border-border text-center">
                      {member.middle}
                    </td>
                    <td className="py-2 px-4 border-b border-border text-center">
                      {member.long}
                    </td>
                    <td className="py-2 px-4 border-b border-border text-center">
                      {member.category}
                    </td>
                    <td className="py-2 px-4 border-b border-border text-center">
                      {member.gender}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ))}
      </div>
    </div>
  );
};
