

import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";
import { EditChallengeForm } from "./EditChallengeForm";
import { GetChallengeGeneralData } from "./actions";

export default async function EditChallenge({params}:{params:Promise<{id:string}>}){
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const {id}=await params;
    const response=await GetChallengeGeneralData(id);
    if (!response.success){
        const err=response.error;
        return {success:false,message:err};    
    }
    const data=await response.data;
    
    return <EditChallengeForm id={id} title={data.title} description={data.description}/>;
}
