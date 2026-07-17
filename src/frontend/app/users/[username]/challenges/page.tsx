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
    await GetChallengesRequest({username});
    return <ShowChallenges username={username}/>;    
}