import { redirect } from "next/navigation";
import { getSession } from "@/lib/session";
import { GetChallengeRequest, GetLanguagesSupported } from "./actions";
import  SubmitAnswer  from "./SubmitAnswer";

export default async function ShowChallengePage({params}:{params:Promise<{id:string}>}) {
    //todo: check if viewer is author
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    
    const{id}=await params;
    var res=await GetChallengeRequest({id});
    if (!res.success) {
        return <p>Couldnt get challenge: {res.error}</p>;
    }
    var res2=await GetLanguagesSupported({id});
    if (!res2.success) {
        return <p>Couldnt get challenge: {res2.error}</p>;
    }
    
    return <SubmitAnswer challenge={res.challenge} manifests={res2.manifests}/>;    
}