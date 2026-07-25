import { redirect } from "next/navigation";
import { getSession } from "@/lib/session";
import { GetChallengeRequest } from "./actions";
import { ShowChallenge } from "./ShowChallenge";
export default async function ShowChallengePage({params}:{params:Promise<{id:string}>}) {
    //todo: check if viewer is author
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    
    const{id}=await params;
    var res=await GetChallengeRequest({id});
    
    if (!res.success) {
    return <p>Couldnt get challenges: {res.error}</p>;
    }
    
    return <ShowChallenge challenge={res.challenge}/>;    
}