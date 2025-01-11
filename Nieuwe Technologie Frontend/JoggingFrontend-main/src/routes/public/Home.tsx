import NavMenu from "../../components/nav/HomeNavBar";
import Wedstrijden from "./wedstrijden/Wedstrijden";
import VeelgesteldeVragen from "./veelgesteldeVragen/VeelgesteldeVragen";
import Klassementen from "./klassementen/Klassementen";
import Uitslagen from "./uitslagen/Uitslagen";
import OverOns from "./overOns/OverOns";
import React, { useRef } from "react";
import Footer from "@/components/footer/Footer";
import heroFoto from "../../assets/images/heroFoto.jpg";
import { Clubs } from "./clubs/clubs";

export default function Home() {
  const section1Ref = useRef<HTMLDivElement>(null);
  const section2Ref = useRef<HTMLDivElement>(null);
  const section3Ref = useRef<HTMLDivElement>(null);
  const section4Ref = useRef<HTMLDivElement>(null);
  const section5Ref = useRef<HTMLDivElement>(null);
  const section6Ref = useRef<HTMLDivElement>(null);

  const scrollToSection = (sectionRef: React.RefObject<HTMLDivElement>) => {
    sectionRef.current?.scrollIntoView({ behavior: "smooth" });
  };
  return (
    <div className="flex flex-col items-center w-full min-h-screen">
      <header className="w-full">
        <div className="fixed z-40 w-full px-3 mt-4 md:px-6">
          <NavMenu
            scrollToSection={scrollToSection}
            sectionRefs={{
              section1Ref,
              section2Ref,
              section3Ref,
              section4Ref,
              section5Ref,
              section6Ref,
            }}
          />
        </div>
      </header>
      <main className="w-full h-full">
        <div className="relative w-full h-full select-none -z-10 ">
          <img
            className="object-cover w-screen h-screen"
            // src='https://wnhnvxvcynrkidmptsga.supabase.co/storage/v1/object/public/react_images/home_banner.jpg'
            src={heroFoto}
            alt=""
          />
          <div className="absolute top-0 flex items-end justify-center w-full h-full bg-gradient-to-b from-transparent to-background">
            <p className="p-6 font-semibold text-center mb-36 text-7xl md:text-8xl">
              Evergemse Joggings
            </p>
          </div>
        </div>
        <div className="relative w-full h-full p-6 md:px-12">
          <div
            className="flex flex-col w-full max-w-full gap-2 pt-20 mx-auto md:max-w-4xl"
            ref={section1Ref}
          >
            <Wedstrijden />
          </div>

          <div
            className="flex flex-col w-full max-w-full gap-2 pt-20 mx-auto md:max-w-4xl"
            ref={section2Ref}
          >
            <Klassementen />
          </div>

          {
            <div
              className="flex flex-col w-full h-screen max-w-4xl gap-2 pt-20 mx-auto"
              ref={section3Ref}
            >
              <Uitslagen />
            </div>
          }

          <div
            className="flex flex-col w-full max-w-4xl gap-2 pt-20 mx-auto gap-y-6"
            ref={section5Ref}
          >
            <OverOns />
          </div>

          <div
            className="flex flex-col w-full max-w-4xl gap-2 pt-20 mx-auto gap-y-6"
            ref={section4Ref}
          >
            <VeelgesteldeVragen />
          </div>
          <div
            className="flex flex-col w-full max-w-4xl gap-2 pt-20 mx-auto gap-y-6"
            ref={section6Ref}
          >
            <Clubs />
          </div>
        </div>
      </main>
      <Footer />
    </div>
  );
}
