

import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";
import ViewLanguages from "./ViewLanguages";
import { GetLanguagesSupported } from "./actions";
import { Manifest } from "@/lib/ClassTypes";
export default async function ViewChallengeLanguages({params}:{params:Promise<{id:string}>}){
   
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const {id}=await params;
    const response=await GetLanguagesSupported(id);
    if (!response.success){
        return <p>Couldnt get languages supported for this challenge.</p>
    }
    
    return <ViewLanguages id={id} manifests={response.manifests}/>;
}