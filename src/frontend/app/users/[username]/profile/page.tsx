import { redirect } from "next/navigation";
import { getSession } from "@/lib/session";
import { GetProfile } from "./actions";
import { ShowProfile } from "./ShowProfile";
import { Profile } from "@/lib/ClassTypes";
export default async function ProfilePage({params}:{params:Promise<{username:string}>}) {
    
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    
    const{username}=await params;
    var res=await GetProfile({username});
    
    if (!res.success) {
    return <p>Couldnt get challenges: {res.error}</p>;
    }
    
    return <ShowProfile username={username} profile={res.profile}/>;    
}