import { redirect } from "next/navigation";
import { getSession } from "@/lib/session";
import { ShowRanking } from "./ShowRanking";
import { GetLanguages, GetRankingRequest } from "./actions";
export default async function ShowRankingPage() {
    //todo: check if viewer is author
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    
    var res2=await GetLanguages();
    if (!res2.success) {
    return <p>Couldnt get languages: {res2.error}</p>;
    }
    
    return <ShowRanking languages={res2.languages}/>;    
}