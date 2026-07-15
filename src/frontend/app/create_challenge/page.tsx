
import { ChallengeForm } from "./ChallengeForm";
import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";
export default async function CreateChallenge(){
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    return <ChallengeForm/>;
}
