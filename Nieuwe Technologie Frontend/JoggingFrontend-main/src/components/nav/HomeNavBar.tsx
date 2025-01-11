import { Button } from "@/components/ui/button";
import {
  NavigationMenu,
  NavigationMenuContent,
  NavigationMenuItem,
  NavigationMenuLink,
  NavigationMenuList,
  NavigationMenuTrigger,
} from "@/components/ui/navigation_menu";
import { Ghost, Menu } from "lucide-react";
import { Link, useNavigate } from "react-router-dom";
import AccountDropDown from "../dropdowns/AccountDropDown";
import { ModeToggle } from "@/components/ModeToggle";
import { RefObject, useRef } from "react";
import {
  Sheet,
  SheetClose,
  SheetContent,
  SheetTrigger,
} from "@/components/ui/sheet";

interface SectionRefs {
  section1Ref: RefObject<HTMLDivElement>;
  section2Ref: RefObject<HTMLDivElement>;
  section3Ref: RefObject<HTMLDivElement>;
  section4Ref: RefObject<HTMLDivElement>;
  section5Ref: RefObject<HTMLDivElement>;
  section6Ref: RefObject<HTMLDivElement>;
}

interface NavMenuProps {
  scrollToSection: (sectionRef: RefObject<HTMLDivElement>) => void;
  sectionRefs: SectionRefs;
}

const HomeNavBar: React.FC<NavMenuProps> = ({
  scrollToSection,
  sectionRefs,
}) => {
  const navigate = useNavigate();
  const navData = [
    { name: "Wedstrijden", ref: sectionRefs.section1Ref },
    {
      name: "Klassementen",
      navigate: "/klassementen",
      ref: sectionRefs.section2Ref,
    },
    {
      name: "Uitslagen",
      navigate: "/uitslagen",
      ref: sectionRefs.section3Ref,
    },
    { name: "Over ons", ref: sectionRefs.section5Ref },
    { name: "Veelgestelde vragen", ref: sectionRefs.section4Ref },
    { name: "Clubs", navigate: "/clubs", ref: sectionRefs.section6Ref },
  ];

  const handleClick = (item: {
    name: string;
    ref: RefObject<HTMLDivElement>;
    navigate?: string;
  }) => {
    if (item.navigate) {
      navigate(item.navigate);
    } else {
      scrollToSection(item.ref);
    }
  };

  return (
    <>
      {/* Mobile Navigation Drawer */}
      <div className="flex items-center justify-between w-full lg:hidden">
        <Sheet>
          <SheetTrigger asChild>
            <Button variant="ghost" size="icon">
              <Menu />
            </Button>
          </SheetTrigger>
          <SheetContent side="left">
            <nav className="grid gap-y-6">
              {navData.map((item) => (
                <SheetClose key={item.name} asChild>
                  <Button
                    key={item.name}
                    variant="ghost"
                    onClick={() => handleClick(item)}
                  >
                    <p className="w-full text-lg font-medium text-left">
                      {item.name}
                    </p>
                  </Button>
                </SheetClose>
              ))}
            </nav>
          </SheetContent>
        </Sheet>
        <div className="flex gap-3">
          <ModeToggle />
          <AccountDropDown />
        </div>
      </div>

      {/* Desktop Navigation Bar */}
      <div className="flex items-center justify-center invisible gap-3 px-4 py-2 mx-auto rounded-full lg:visible w-max dark:bg-slate-700/50 bg-slate-50/50 backdrop-blur-sm">
        <Link to="/" className="pb-0.5 font-bold ">
          Evergemse Joggings
        </Link>

        <NavigationMenu>
          <NavigationMenuList>
            {navData.map((item) => (
              <NavigationMenuItem key={item.name}>
                <Button
                  className="rounded-full"
                  variant="ghost"
                  onClick={() => handleClick(item)}
                >
                  {item.name}
                </Button>
              </NavigationMenuItem>
            ))}
          </NavigationMenuList>
        </NavigationMenu>

        <ModeToggle />

        <AccountDropDown />
      </div>
    </>
  );
};

export default HomeNavBar;
