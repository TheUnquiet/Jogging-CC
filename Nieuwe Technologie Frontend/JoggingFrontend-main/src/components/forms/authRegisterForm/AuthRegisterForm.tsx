import React, { useState, useRef, useEffect } from "react";
import { AddressInfoComponent } from "./Sections/AddressForm";
import { ContactInfoComponent } from "./Sections/ContactInfoForm";
import { PersonalInfoComponent } from "./Sections/PersonalInfoForm";
import { Button } from "@/components/ui/button";
import { AddressInfo } from "./Sections/AddressFormSetup";
import { ContactInfo } from "./Sections/ContactInfoFormSetup";
import { PersonalInfo } from "./Sections/PersonalInfoFormSetup";
import { useAuth } from "@/routes/auth/context/AuthProvider";
import { toast } from "sonner";
import { AxiosError } from "axios";
import { Icons } from "@/lib/Icons.tsx";

const clubsList = ["Kozirunners", "Lucky runners", "Club 3", "Club 4"]; // Voeg hier de lijst met clubs toe

export function AuthRegisterForm() {
  const [addressData, setAddressData] = useState<Partial<AddressInfo>>({});
  const [contactData, setContactData] = useState<Partial<ContactInfo>>({});
  const [personalData, setPersonalData] = useState<Partial<PersonalInfo>>({});
  const [isChecked, setIsChecked] = useState<boolean>(false);
  const [visibleDiv, setVisibleDiv] = useState(0);
  const totalDivs = 3;
  const { register, isLoading } = useAuth();

  const addressValidationRef = useRef<() => Promise<boolean>>(() =>
    Promise.resolve(true)
  );
  const contactValidationRef = useRef<() => Promise<boolean>>(() =>
    Promise.resolve(true)
  );
  const personalValidationRef = useRef<() => Promise<boolean>>(() =>
    Promise.resolve(true)
  );
  const validateCurrentForm = useRef<() => Promise<boolean>>(() =>
    Promise.resolve(true)
  );

  useEffect(() => {
    switch (visibleDiv) {
      case 0:
        validateCurrentForm.current = contactValidationRef.current;
        break;
      case 1:
        validateCurrentForm.current = personalValidationRef.current;
        break;
      case 2:
        validateCurrentForm.current = addressValidationRef.current;
        break;
      default:
        validateCurrentForm.current = () => Promise.resolve(true);
        break;
    }
  }, [visibleDiv]);

  const handleNextClick = async () => {
    const isValid = await validateCurrentForm.current();
    if (isValid && visibleDiv < totalDivs - 1) {
      setVisibleDiv((current) => current + 1);
    }
  };

  const handleAddressSubmit = (data: AddressInfo) => {
    setAddressData(data);
  };

  const handleContactSubmit = (data: ContactInfo) => {
    setContactData(data);
  };

  const handlePersonalSubmit = (data: PersonalInfo) => {
    setPersonalData(data);
  };

  const onSubmit = async () => {
    const isValid = await validateCurrentForm.current();
    if (isValid) {
      const finalPersonData = {
        firstName: personalData.firstName || "",
        lastName: personalData.lastName || "",
        gender: personalData.gender || "V",
        birthDate: personalData.birthday || new Date().toISOString(),
        club: personalData.club || "",
        address: {
          street: addressData.street || "",
          zipCode: addressData.zipCode || "",
          city: addressData.city || "",
          houseNumber: addressData.houseNumber || "",
        },
      };

      console.log("Final Form Data:", finalPersonData);

      if (!contactData.email || !contactData.password) {
        console.error("Email and password must be provided.");
        toast("Email and password must be provided.");
        return;
      }

      try {
        await register(
          contactData.email,
          contactData.password,
          finalPersonData
        );
        console.log("Registration successful");
      } catch (error) {
        if (error) {
          if (error === "This email address is already registered.") {
            console.error("Registratie mislukt: E-mailadres bestaat al.");
            toast(
              "Registratie mislukt: Er bestaat al een account met dit e-mailadres."
            );
          } else {
            console.error("Registratie mislukt:", error);
            toast("Registratie mislukt: Onbekende fout");
          }
        } else {
          const errorMessage = (error as Error).message;
          console.error("Registratie mislukt:", errorMessage);
          toast("Registratie mislukt: " + errorMessage);
        }
      }
    } else {
      toast(
        "Validatie mislukt, kon de gegevens niet indienen. Ongekende fout."
      );
    }
  };

  // Helper function to check if error is an AxiosError
  function isAxiosError(error: unknown): error is AxiosError {
    return (error as AxiosError).isAxiosError !== undefined;
  }

  return (
    <>
      <div className="flex flex-col space-y-2 text-center">
        <h1 className="text-2xl font-semibold tracking-tight">Registreer</h1>
        <p className="text-sm text-muted-foreground">
          Voer hier uw gegevens in om een account aan te maken.
        </p>
      </div>

      <div>
        <div className={`${visibleDiv === 0 ? "block" : "hidden"}`}>
          <ContactInfoComponent
            onDataChange={handleContactSubmit}
            setValidateCurrentForm={contactValidationRef}
          />
        </div>
        <div className={`${visibleDiv === 1 ? "block" : "hidden"}`}>
          <PersonalInfoComponent
            onDataChange={handlePersonalSubmit}
            setValidateCurrentForm={personalValidationRef}
          />
          <div className="mt-4">
            <label
              htmlFor="club"
              className="block text-sm font-medium text-gray-700"
            >
              Club
            </label>
            <select
              id="club"
              value={personalData.club}
              onChange={(e) =>
                setPersonalData({ ...personalData, club: e.target.value })
              }
              className="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
            >
              <option value="">Select a club</option>
              {clubsList.map((club) => (
                <option key={club} value={club}>
                  {club}
                </option>
              ))}
            </select>
          </div>
        </div>
        <div className={`${visibleDiv === 2 ? "block" : "hidden"}`}>
          <AddressInfoComponent
            onDataChange={handleAddressSubmit}
            setValidateCurrentForm={addressValidationRef}
            setIsChecked={setIsChecked}
            isChecked={isChecked}
          />
        </div>
        <div className="flex justify-between mt-3 space-x-1">
          {visibleDiv > 0 && (
            <Button
              disabled={isLoading}
              onClick={() =>
                setVisibleDiv((current) => Math.max(current - 1, 0))
              }
            >
              <svg
                width="15"
                height="15"
                viewBox="0 0 15 15"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M8.84182 3.13514C9.04327 3.32401 9.05348 3.64042 8.86462 3.84188L5.43521 7.49991L8.86462 11.1579C9.05348 11.3594 9.04327 11.6758 8.84182 11.8647C8.64036 12.0535 8.32394 12.0433 8.13508 11.8419L4.38508 7.84188C4.20477 7.64955 4.20477 7.35027 4.38508 7.15794L8.13508 3.15794C8.32394 2.95648 8.64036 2.94628 8.84182 3.13514Z"
                  fill="currentColor"
                  fillRule="evenodd"
                  clipRule="evenodd"
                ></path>
              </svg>
            </Button>
          )}
          {visibleDiv < totalDivs - 1 && (
            <Button
              disabled={isLoading}
              className="w-full"
              onClick={handleNextClick}
            >
              {isLoading ? (
                <Icons.spinner className="w-4 h-4 mr-2 animate-spin" />
              ) : (
                "Volgende"
              )}
            </Button>
          )}
          {visibleDiv === totalDivs - 1 && (
            <Button
              disabled={isLoading || !isChecked}
              className="w-full"
              onClick={onSubmit}
            >
              {isLoading ? (
                <Icons.spinner className="w-4 h-4 mr-2 animate-spin" />
              ) : (
                "Registreer"
              )}
            </Button>
          )}
        </div>
      </div>
    </>
  );
}
