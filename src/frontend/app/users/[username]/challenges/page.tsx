import { redirect } from "next/navigation";
import { getSession } from "@/lib/session";
import { ShowChallenges } from "./ShowChallenges";
import { GetChallengesRequest } from "./actions";
export default async function ShowChallengesPage({params}:{params:Promise<{username:string}>}) {
    //todo: check if viewer is author
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    
    const{username}=await params;
    var res=await GetChallengesRequest({username});
    
    if (!res.success) {
    return <p>Couldnt get challenges: {res.error}</p>;
    }
    return <ShowChallenges username={username} challenges={res.challenges}/>;    
}