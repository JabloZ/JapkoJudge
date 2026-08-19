import { redirect } from "next/navigation";
import { getSession, isAdmin } from "@/lib/session";
import { GetChallengesRequest } from "./actions";
import { ShowUnverifiedChallenges } from "./ShowUnverifiedChallenges";
export default async function ShowUnverifiedChallengesPage() {
    //todo: check if viewer is author
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const isUserAdmin= await isAdmin();
    if (!isUserAdmin){
        redirect("/"); 
    }
    
    //CHECK IF ADMIN TODO
    
    var res=await GetChallengesRequest();
    
    if (!res.success) {
    return <p>Couldnt get challenges: {res.error}</p>;
    }
    
    return <ShowUnverifiedChallenges challenges={res.challenges}/>;    
}