'use server';
import { getSession, getToken } from "@/lib/session";
import { redirect } from "next/navigation";
export async function GetChallengesRequest({username}:{username:string}){
    
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const token=getToken()
    if(!token){
        return {error:"no token"};
    }

    const response=await fetch(`${process.env.BACKEND_URL}/api/users/${username}/challenges`,{
        method:"GET",
        headers:{
            "Authorization":`Bearer ${token}`
        }
    })
    
    if (!response.ok){
        return {success:false,error:response.status};
    }
    const data=await response.json();
    //todo - rework sending back responses from backend because now all user data is being returned(hash included)
    console.log(data.challenges[0]);
    /* snippet przykladowego
        var challenges = await db.Challenges
        .Where(c => c.OwnerId == owner.Id)
        .Select(c => new ChallengeDto
        {
            Id = c.Id,
            Title = c.Title,
            Difficulty = c.Difficulty,
            Description = c.Description,
            Verified = c.Verified,
            OwnerUsername = c.User!.Username
        })
        .ToListAsync();

    return Results.Ok(challenges);
    */
    return{success:true,data};

}