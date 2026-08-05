

import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";
import { ShowSubmissions } from "./ShowSubmissions";

import { GetSubmissions } from "./actions";
export default async function ViewChallengeLanguages({params}:{params:Promise<{id:string}>}){
   
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const OwnId=session.id;
    const {id}=await params;
    const response=await GetSubmissions(id);
    if (!response.success){
        return <p>Couldnt get Submissions.</p>
    }
    
    return <ShowSubmissions id={id} own_id={OwnId} submissions={response.submissions}/>;
}